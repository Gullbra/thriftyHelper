import { useContext } from 'react'
import { useOutletContext } from "react-router-dom"

import { Sidebar, Main } from "../Layout"
import { DataContext } from '../util/context'

export const IngredientsView = () => {
  const [ showSidebar, setShowSidebar ] = useOutletContext() as [ boolean, React.Dispatch<React.SetStateAction<boolean>> ]

  const ingredientsContext = useContext(DataContext).ingredients

  return(
    <>
      <Sidebar showSidebar={showSidebar}>
        {ingredientsContext.categories.map(category => (
          <p key={category}>{category}</p>
        ))}
      </Sidebar>

      <Main showSidebar={showSidebar}>
        <p>IngrediensView</p>
      </Main>
    </>
  )
}