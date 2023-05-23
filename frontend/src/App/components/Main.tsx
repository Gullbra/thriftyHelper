import '../styles/components/main.css'

export const Main = ({children, showSidebar}:  {children: React.ReactNode, showSidebar: boolean}) => (
  <main className={`site__main --dev-border${showSidebar ? ' --sidebar-open__main-margin': ''}`}> 
    {children}
  </main>
)