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
          <Link className={`site-header__nav__nav-links`} to={"/g"}>Dev:404</Link>
          <Link className={`site-header__nav__nav-links${currentLocation.split('/')[1] === '' ? ' --current-loc': ''}`} to={"/"}>Home</Link>
          <Link className={`site-header__nav__nav-links${currentLocation.split('/')[1] === 'ingredients' ? ' --current-loc': ''}`} to={"/ingredients"}>Ingredients</Link>
          <Link className={`site-header__nav__nav-links${currentLocation.split('/')[1] === 'recipies' ? ' --current-loc': ''}`} to={"/recipies"}>Recipies</Link>
        </nav>
      </header>

      {children 
        ? <main className={`site__main --justify-c-center`}> {children} </main>
        : hasSidebar 
          ? <Outlet context={[ showSidebar, setShowSidebar ]} />
          : <main className={`site__main --justify-c-center`}> <Outlet /> </main>}
          
      <footer className="site__footer">Created by Gullbra</footer>
    </>
  )
}
