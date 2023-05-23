import { useContext } from 'react'
import { useOutletContext } from "react-router-dom"

import '../styles/views/ingredients.css'

import { Main } from "../components/Main"
import { Sidebar } from "../components/Sidebar"
import { DataContext } from '../util/context'

export const IngredientsView = () => {
  const [ showSidebar, setShowSidebar, filters ] = useOutletContext() as [ boolean, React.Dispatch<React.SetStateAction<boolean>>, string[] ]

  const ingredientsContext = useContext(DataContext).ingredients
  // const ingredientsContext = (() =>{
  //   if (filters.length === 0) {
  //     return useContext(DataContext).ingredients
  //   }
  //   return useContext(DataContext).ingredients.filter(ingredient => ingredient)
  // }) ()

  return(
    <>
      <Sidebar showSidebar={showSidebar}>
        {ingredientsContext.categories.map(category => (
          <p key={category}>{category}</p>
        ))}
      </Sidebar>

      <Main showSidebar={showSidebar}>
        <flex-wrapper class='ingredient-view__main__mode-choice-container'>
          <h4>Ingredients List</h4>
          {/* //! Auth protected? */}
          <h4>Add new Ingredient</h4>
        </flex-wrapper>

        <div className="ingredients-view__main__list-wrapper">
          <div>name</div>
          <div>hey</div>
          <div>hey</div>
          <div>hey</div>
          {ingredientsContext.ingredientsList.map(ingredient => (
            <div className='' key={ingredient.id}>
              <p>{ingredient.name}</p>
              <p>{ingredient.name}</p>
              <p>{ingredient.name}</p>
              <p>{ingredient.name}</p>
            </div>
          ))}
        </div>
      </Main>
    </>
  )
}