import { useState } from "react"
import { Outlet, Link, useLocation, useOutletContext } from "react-router-dom"

import { HamburgerMenu } from "./components/HamburgerMenu"

import './styles/layout.css'

export const Layout = ({children}: {children: React.ReactNode}) => {
  const [ showSidebar, setShowSidebar ] = useState<boolean>(true)
  const currentLocation = useLocation().pathname
  const isHomePage = currentLocation === '/'

  console.log({children, typeof: typeof children})
  return (
    <>
      <header className="site__header">
        <flex-wrapper class="site-header__header">
          {!isHomePage && !children && (
            <HamburgerMenu callbackFunc={() => setShowSidebar(prev => !prev)}/>
          )}
          <h1 className="header__header">Thrifty Helper</h1>
        </flex-wrapper>
        
        <nav className="site-header__nav-bar">
          <Link className="site-header__nav__nav-links" to={"/"}>Home</Link>
          <Link className="site-header__nav__nav-links" to={"/ingredients"}>Ingredients</Link>
          <Link className="site-header__nav__nav-links" to={"/recipies"}>Recipies</Link>
        </nav>
      </header>

      {children 
        ? (
          <main className={`site__main${showSidebar ? ' --sidebar-open__main-margin': ''}`}> {children} </main>
        ) : (
          <Outlet context={[ showSidebar, setShowSidebar ]} />
        )}

      <footer className="site__footer">Created by Gullbra</footer>
    </>
  )
}

export const SidebarAndMain = (
  //{showSidebar, setShowSidebar}:  {showSidebar: boolean, setShowSidebar: React.Dispatch<React.SetStateAction<boolean>>}
  {children}: {children: React.ReactNode}
) => {
  const [ showSidebar, setShowSidebar ] = useOutletContext() as [ boolean, React.Dispatch<React.SetStateAction<boolean>> ]
  const currentLocation = useLocation().pathname
  const isHomePage = currentLocation === '/'

  return (
    <>
      {!isHomePage && (
        <aside className={`site__sidebar${showSidebar ? ' --sidebar-open__sidebar-width': ''}`}>
        </aside>
      )}

      <main className={`site__main${showSidebar ? ' --sidebar-open__main-margin': ''}`}> 
        <Outlet/>
        {children}
      </main>
    </>
  )
}