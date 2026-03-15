/** @type {import('tailwindcss').Config} */
export default {
  // Only generate Tailwind classes actually used in source files
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  // Prevent Tailwind's base reset from conflicting with Ant Design
  corePlugins: {
    preflight: false,
  },
  theme: {
    extend: {},
  },
  plugins: [],
}
