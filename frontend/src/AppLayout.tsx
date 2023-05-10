import { useState } from "react"
import { HamburgerMenu } from "./components/HamburgerMenu"

import './styles/layout.css'




export const Layout = ({children}: {children: React.ReactNode}) => {

  const [ showSidebar, setShowSidebar ] = useState<boolean>(true)

  return (
    <>
      <header className="site__header">
        <flex-wrapper class="site-header__header">
          <HamburgerMenu callbackFunc={() => setShowSidebar(prev => !prev)}/>
          <h1 className="header__header">Thrifty Helper</h1>
        </flex-wrapper>
        
        <nav className="site-header__nav-bar">
          navbar-links
        </nav>
      </header>

      <aside className={`site__sidebar${showSidebar ? ' --sidebar-open__sidebar-width': ''}`}></aside>

      <main className={`site__main${showSidebar ? ' --sidebar-open__main-margin': ''}`}> {children} </main>

      <footer className="site__footer"></footer>
    </>
  )
}