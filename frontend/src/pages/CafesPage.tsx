import { useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { Button, Input, message } from 'antd'
import { PlusOutlined } from '@ant-design/icons'
import { AgGridReact } from 'ag-grid-react'
import type { ColDef, ICellRendererParams } from 'ag-grid-community'
import 'ag-grid-community/styles/ag-grid.css'
import 'ag-grid-community/styles/ag-theme-alpine.css'
import { useCafes, useDeleteCafe } from '../hooks/useCafes'
import { Cafe } from '../types'
import PageHeader from '../components/PageHeader'
import TableActionButtons from '../components/TableActionButtons'
import QueryStateWrapper from '../components/QueryStateWrapper'

export default function CafesPage() {
  const navigate = useNavigate()
  const [searchParams, setSearchParams] = useSearchParams()
  const [locationFilter, setLocationFilter] = useState(searchParams.get('location') ?? '')

  const { data: cafes = [], isLoading, isError, error, refetch } = useCafes(locationFilter || undefined)
  const deleteCafe = useDeleteCafe()

  function handleFilterChange(value: string) {
    setLocationFilter(value)
    value ? setSearchParams({ location: value }) : setSearchParams({})
  }

  async function handleDelete(cafe: Cafe) {
    try {
      await deleteCafe.mutateAsync(cafe.id)
      message.success(`"${cafe.name}" deleted.`)
    } catch (err) {
      message.error(err instanceof Error ? err.message : 'Failed to delete café.')
    }
  }

  function ActionsCellRenderer({ data }: ICellRendererParams<Cafe>) {
    if (!data) return null
    return (
      <TableActionButtons
        onEdit={() => navigate(`/cafes/edit/${data.id}`)}
        onDelete={() => handleDelete(data)}
        deleteConfirmTitle={`Delete "${data.name}"?`}
        deleteConfirmContent="This will permanently delete the café and all its employees."
      />
    )
  }

  // Renders the employee count as a clickable badge.
  // Uses navigate() so it stays within the React Router SPA — no full-page reload.
  function EmployeeCountCellRenderer({ value, data }: ICellRendererParams<Cafe>) {
    if (!data) return null
    return (
      <span
        className="stat-badge"
        style={{ cursor: 'pointer' }}
        onClick={() => navigate(`/employees?cafe=${data.id}`)}
      >
        {value} staff
      </span>
    )
  }

  const columns: ColDef<Cafe>[] = [
    {
      headerName: 'Logo',
      field: 'logo',
      width: 72,
      sortable: false,
      suppressHeaderMenuButton: true,
      cellRenderer: ({ value }: ICellRendererParams<Cafe>) => {
        if (value) return <img src={value} alt="logo" style={{ width: 36, height: 36, objectFit: 'cover', borderRadius: 6, border: '1px solid #e2e8f0' }} />
        return <span style={{ color: '#94a3b8', fontSize: 22 }}>☕</span>
      },
    },
    { headerName: 'Name', field: 'name', flex: 1, suppressHeaderMenuButton: true },
    { headerName: 'Description', field: 'description', flex: 2, suppressHeaderMenuButton: true  },
    {
      headerName: 'Employees',
      field: 'employees',
      width: 120,
      suppressHeaderMenuButton: true,
      // React cell renderer so navigate() works — a plain <a href> would reload the page
      cellRenderer: EmployeeCountCellRenderer,
    },
    { headerName: 'Location', field: 'location', flex: 1, suppressHeaderMenuButton: true },
    { headerName: 'Actions', width: 225, suppressHeaderMenuButton: true , sortable: false, cellRenderer: ActionsCellRenderer },
  ]

  return (
    <div className="page-card">
      <PageHeader
        title="Cafés"
        subtitle={!isLoading && !isError ? `${cafes.length} café${cafes.length !== 1 ? 's' : ''} found` : undefined}
        actions={
          <>
            <Input.Search
              placeholder="Filter by location…"
              value={locationFilter}
              onChange={(e) => handleFilterChange(e.target.value)}
              onSearch={handleFilterChange}
              allowClear
              style={{ width: 220 }}
            />
            <Button type="primary" icon={<PlusOutlined />} onClick={() => navigate('/cafes/new')}>
              Add New Café
            </Button>
          </>
        }
      />

      <QueryStateWrapper
        isLoading={isLoading}
        isError={isError}
        error={error as Error | null}
        isEmpty={cafes.length === 0}
        emptyTitle="No cafés yet"
        emptyDescription={locationFilter ? `No cafés found in "${locationFilter}".` : 'Get started by adding your first café.'}
        onAddNew={() => navigate('/cafes/new')}
        addNewLabel="Add First Café"
        onRetry={refetch}
      >
        <div className="ag-theme-alpine style={{ width: '100%' }}">
          <AgGridReact
            rowData={cafes}
            columnDefs={columns}
            domLayout="autoHeight"
            defaultColDef={{ sortable: true, resizable: true, minWidth: 80 }}
            onGridReady={(params) => params.api.sizeColumnsToFit()}
            onGridSizeChanged={(params) => params.api.sizeColumnsToFit()}
          />
        </div>
      </QueryStateWrapper>
    </div>
  )
}
