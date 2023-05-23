import { useContext } from 'react'
import { useOutletContext } from "react-router-dom"

import '../styles/views/ingredients.css'

import { Main } from "../components/Main"
import { Sidebar } from "../components/Sidebar"
import { DataContext } from '../util/context'

export const IngredientsView = () => {
  const [ 
    showSidebar, 
    //setShowSidebar, 
    //filters 
  ] = useOutletContext() as [ boolean, React.Dispatch<React.SetStateAction<boolean>>, string[] ]

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

          <div className="list-wrapper__grid-wrapper">

            <div className='--grid-header'>name</div>
            <div className='--grid-header'>unit</div>
            <div className='--grid-header'>{"Energy/unit"}</div>
            <div className='--grid-header'>{"Protein/unit"}</div>
            <div className='--grid-header'>{"price/unit"}</div>
            {ingredientsContext.ingredientsList.map(ingredient => (
              <>
                <p className='--grid-entries' key={ingredient.id + ingredient.name}>{ingredient.name}</p>
                <p className='--grid-entries' key={ingredient.id + ingredient.unit}>{ingredient.unit}</p>
                <p className='--grid-entries' key={ingredient.id + ingredient.energyPerUnit}>{ingredient.energyPerUnit}</p>
                <p className='--grid-entries' key={ingredient.id + ingredient.proteinPerUnit}>{ingredient.proteinPerUnit}</p>
                <p className='--grid-entries' key={ingredient.id + ingredient.pricePerUnit}>{ingredient.pricePerUnit}</p>
              </>
            ))}
          </div>
        </div>
      </Main>
    </>
  )
}