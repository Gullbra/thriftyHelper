import { useState } from "react"
import { Outlet, Link, useLocation } from "react-router-dom"

import { HamburgerMenu } from "./components/HamburgerMenu"

import './styles/layout.css'

export const Layout = () => {
  const [ showSidebar, setShowSidebar ] = useState<boolean>(true)
  const currentLocation = useLocation().pathname
  const isHomePage = currentLocation === '/'

  return (
    <>
      <header className="site__header">
        <flex-wrapper class="site-header__header">
          {!isHomePage && (
            <HamburgerMenu callbackFunc={() => setShowSidebar(prev => !prev)}/>
          )}
          <h1 className="header__header">Thrifty Helper</h1>
        </flex-wrapper>
        
        <nav className="site-header__nav-bar">
          <Link className="site-header__nav__nav-links" to={"/test"}>dev:test</Link>
          <Link className="site-header__nav__nav-links" to={"/"}>dev:home</Link>
          <Link className="site-header__nav__nav-links" to={"/ingredients"}>Ingredients</Link>
          <Link className="site-header__nav__nav-links" to={"/recepies"}>Recepies</Link>
        </nav>
      </header>

      {!isHomePage && (
        <aside className={`site__sidebar${showSidebar ? ' --sidebar-open__sidebar-width': ''}`}></aside>
      )}

      <main className={`site__main${showSidebar ? ' --sidebar-open__main-margin': ''}`}> <Outlet/> </main>

      <footer className="site__footer">Created by Gullbra</footer>
    </>
  )
}