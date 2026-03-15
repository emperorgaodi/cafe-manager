import { useNavigate, useSearchParams } from 'react-router-dom'
import { Button, message } from 'antd'
import { PlusOutlined } from '@ant-design/icons'
import { AgGridReact } from 'ag-grid-react'
import type { ColDef, ICellRendererParams } from 'ag-grid-community'
import dayjs from 'dayjs'
import relativeTime from 'dayjs/plugin/relativeTime'
import 'ag-grid-community/styles/ag-grid.css'
import 'ag-grid-community/styles/ag-theme-alpine.css'
import { useDeleteEmployee, useEmployees } from '../hooks/useEmployees'
import { Employee } from '../types'
import PageHeader from '../components/PageHeader'
import TableActionButtons from '../components/TableActionButtons'
import QueryStateWrapper from '../components/QueryStateWrapper'

dayjs.extend(relativeTime)

export default function EmployeesPage() {
  const navigate = useNavigate()
  const [searchParams, setSearchParams] = useSearchParams()
  const cafeId = searchParams.get('cafe') ?? undefined

  const { data: employees = [], isLoading, isError, error, refetch } = useEmployees(cafeId)
  const deleteEmployee = useDeleteEmployee()

  async function handleDelete(employee: Employee) {
    try {
      await deleteEmployee.mutateAsync(employee.id)
      message.success(`"${employee.name}" deleted.`)
    } catch (err) {
      message.error(err instanceof Error ? err.message : 'Failed to delete employee.')
    }
  }

  function ActionsCellRenderer({ data }: ICellRendererParams<Employee>) {
    if (!data) return null
    return (
      <TableActionButtons
        onEdit={() => navigate(`/employees/edit/${data.id}`)}
        onDelete={() => handleDelete(data)}
        deleteConfirmTitle={`Delete "${data.name}"?`}
        deleteConfirmContent="This employee will be permanently removed."
      />
    )
  }

  const columns: ColDef<Employee>[] = [
    {
      headerName: 'Employee ID',
      field: 'id',
      width: 130,
      suppressHeaderMenuButton: true,
      cellRenderer: ({ value }: ICellRendererParams<Employee>) => <span className="employee-id">{value}</span>,
    },
    { headerName: 'Name', field: 'name', flex: 1, suppressHeaderMenuButton: true },
    { headerName: 'Email', field: 'emailAddress', flex: 1, suppressHeaderMenuButton: true },
    { headerName: 'Phone', field: 'phoneNumber', width: 120, suppressHeaderMenuButton: true },
    {
      headerName: 'Days Worked',
      field: 'daysWorked',
      width: 130,
      sort: 'desc',
      suppressHeaderMenuButton: true,
      // Dayjs computes a human-readable tooltip: start date + relative time
      tooltipValueGetter: ({ data }) => {
        if (!data?.daysWorked) return 'Not yet assigned'
        const startDate = dayjs().subtract(data.daysWorked, 'day')
        return `Started ${startDate.format('D MMM YYYY')} · ${startDate.fromNow()}`
      },
    },
    {
      headerName: 'Café',
      field: 'cafe',   // Matches the "cafe" field name from the API spec
      flex: 1,
      suppressHeaderMenuButton: true,
      valueFormatter: ({ value }) => value ?? '—',
      cellStyle: ({ value }) => ({
        color: value ? 'var(--color-text)' : 'var(--color-text-muted)',
        fontStyle: value ? 'normal' : 'italic',
      }),
    },
    { headerName: 'Actions', width: 175, sortable: false, suppressHeaderMenuButton: true, cellRenderer: ActionsCellRenderer },
  ]

  return (
    <div className="page-card">
      <PageHeader
        title={cafeId ? 'Employees in Café' : 'All Employees'}
        subtitle={!isLoading && !isError ? `${employees.length} employee${employees.length !== 1 ? 's' : ''}` : undefined}
        actions={
          <>
            {cafeId && <Button onClick={() => setSearchParams({})}>Show All</Button>}
            <Button type="primary" icon={<PlusOutlined />} onClick={() => navigate('/employees/new')}>
              Add New Employee
            </Button>
          </>
        }
      />

      <QueryStateWrapper
        isLoading={isLoading}
        isError={isError}
        error={error as Error | null}
        isEmpty={employees.length === 0}
        emptyTitle="No employees yet"
        emptyDescription="Get started by adding your first employee."
        onAddNew={() => navigate('/employees/new')}
        addNewLabel="Add First Employee"
        onRetry={refetch}
      >
        <div className="ag-theme-alpine style={{ width: '100%' }}">
          <AgGridReact
            rowData={employees}
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
