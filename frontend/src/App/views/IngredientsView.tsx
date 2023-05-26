import { useContext, useState } from 'react'
import { useOutletContext } from "react-router-dom"

import '../styles/views/ingredients.css'

import { Main } from "../components/Main"
import { Sidebar } from "../components/Sidebar"
import { DataContext } from '../util/context'

export const IngredientsView = () => {
  const [ 
    showSidebar, 
    //setShowSidebar, 
  ] = useOutletContext() as [ boolean, React.Dispatch<React.SetStateAction<boolean>>, string[] ]
  const [ categoryFilter, setCategoryFilter ] = useState<Set<string>>(new Set())

  const ingredientsContext = useContext(DataContext).ingredients
  const ingredientsListToShow = (() => {
    if (categoryFilter.size > 0) {
      return ingredientsContext.ingredientsList.filter(ingredient => {
        for (let index = 0; index < ingredient.inCategories.length; index++)
          if (categoryFilter.has(ingredient.inCategories[index]))
            return true
  
        return false
      })
    }

    return ingredientsContext.ingredientsList
  }) ()

  return(
    <>
      <Sidebar showSidebar={showSidebar}>
        {ingredientsContext.categories.map(category => (
          <p 
            className={`sidebar_ingredients-view__categories-toggle${categoryFilter.has(category) ? ' --sidebar-category-toggled': ''}`}
            key={category}
            onClick={() => {
              const newFilters = new Set(categoryFilter)

              categoryFilter.has(category)
                ? newFilters.delete(category)
                : newFilters.add(category)

              console.log(newFilters)

              setCategoryFilter(newFilters);
            }}
          >
            {category}
          </p>
        ))}
      </Sidebar>

      <Main showSidebar={showSidebar}>
        <flex-wrapper class='ingredient-view__main__mode-choice-container'>
          <h4>Ingredients List</h4>
          {/* //! Auth protected? */}
          <h4>Add new Ingredient</h4>
        </flex-wrapper>

        <div className="ingredients-view__main__list-wrapper">
          <table className="list-wrapper__table-element --dev-border">
            <thead>
              <tr>
                <th className='--grid-header'>name</th>
                <th className='--grid-header'>unit</th>
                <th className='--grid-header'>{"Energy/unit"}</th>
                <th className='--grid-header'>{"Protein/unit"}</th>
                <th className='--grid-header'>{"price/unit"}</th>
              </tr>
            </thead>

            <tbody>

              {ingredientsListToShow.map(ingredient => (
                <tr  key={ingredient.id}>
                  <td className='--grid-entries' key={ingredient.id + ingredient.name}>{ingredient.name}</td>
                  <td className='--grid-entries' key={ingredient.id + ingredient.unit}>{ingredient.unit}</td>
                  <td className='--grid-entries' key={ingredient.id + ingredient.energyPerUnit}>{ingredient.energyPerUnit}</td>
                  <td className='--grid-entries' key={ingredient.id + ingredient.proteinPerUnit}>{ingredient.proteinPerUnit}</td>
                  <td className='--grid-entries' key={ingredient.id + ingredient.pricePerUnit}>{ingredient.pricePerUnit}</td>
                </tr>
              ))}
            </tbody>


          </table>
        </div>

        
        {/* Redo as table */}
        {/* <div className="ingredients-view__main__list-wrapper">
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
        </div> */}



      </Main>
    </>
  )
}