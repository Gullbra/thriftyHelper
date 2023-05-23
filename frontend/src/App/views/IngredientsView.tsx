import { useContext } from 'react'
import { useOutletContext } from "react-router-dom"

import '../styles/views/ingredients.css'

import { Main } from "../components/Main"
import { Sidebar } from "../components/Sidebar"
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
        <div className='ingredient-view__main__mode-choice-container --dev-border'>
          <h4>Ingredients List</h4>
          {/* //! Auth protected? */}
          <h4>Add new Ingredient</h4>
        </div>
        <p>IngrediensView</p>
      </Main>
    </>
  )
}