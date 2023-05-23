import '../styles/components/sidebar.css'

export const Sidebar = (
  {children, showSidebar, 
    // setShowSidebar
  }: {children: React.ReactNode, showSidebar: boolean, 
    // setShowSidebar: React.Dispatch<React.SetStateAction<boolean>>
  }
) => (
  <aside className={`site__sidebar${showSidebar ? ' --sidebar-open__sidebar-width': ''}`}>
    {children}
  </aside>
)