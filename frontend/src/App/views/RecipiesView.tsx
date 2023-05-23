import { useContext } from 'react'
import { useOutletContext } from "react-router-dom"

import { Main } from "../components/Main"
import { Sidebar } from "../components/Sidebar"
import { DataContext } from '../util/context'

export const RecipiesView = () => {
  const [ showSidebar, setShowSidebar ] = useOutletContext() as [ boolean, React.Dispatch<React.SetStateAction<boolean>> ]

  const recipiesContext = useContext(DataContext).recipies

  return(
    <>
      <Sidebar showSidebar={showSidebar}>
        {recipiesContext.categories.map(category => (
          <p key={category}>{category}</p>
        ))}
      </Sidebar>

      <Main showSidebar={showSidebar}>
        <p>RecipiesView</p>
      </Main>
    </>
  )
}