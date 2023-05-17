import { useState } from "react"
import { Outlet, Link, useLocation } from "react-router-dom"

import { HamburgerMenu } from "./components/HamburgerMenu"

import './styles/layout.css'

export const Layout = ({children}: {children?: React.ReactNode}) => {
  const [ showSidebar, setShowSidebar ] = useState<boolean>(true)
  const currentLocation = useLocation().pathname
  const hasSidebar = ['ingredients', 'recipies'].includes(currentLocation.split('/')[1])

  return (
    <>
      <header className="site__header">
        <flex-wrapper class="site-header__header">
          {hasSidebar && (
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
        ? <main className={`site__main`}> {children} </main>
        : hasSidebar 
          ? <Outlet context={[ showSidebar, setShowSidebar ]} />
          : <main className={`site__main`}> <Outlet /> </main>}
          
      <footer className="site__footer">Created by Gullbra</footer>
    </>
  )
}

export const Main = ({children, showSidebar}:  {children: React.ReactNode, showSidebar: boolean}) => (
  <main className={`site__main${showSidebar ? ' --sidebar-open__main-margin': ''}`}> 
    {children}
  </main>
)

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
