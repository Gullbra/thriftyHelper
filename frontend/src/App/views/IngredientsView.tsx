import { useContext, useState } from 'react'
import { useOutletContext } from "react-router-dom"

import '../styles/views/ingredients.css'

import { Main } from "../components/Main"
import { Sidebar } from "../components/Sidebar"
import { DataContext } from '../util/context'
import { capitalize } from '../util/capitalize'
import { compareIngredients } from '../util/sorting'

interface ISortingState {
  activeSort: string,
  possibleSort: string [],
  activeOrder: string,
  possibleOrder: string [],
}

export const IngredientsView = () => {
  const [ showSidebar, /* setShowSidebar, */ ] = useOutletContext() as [ boolean, /* React.Dispatch<React.SetStateAction<boolean>> */ ]
  const [ categoryFilter, setCategoryFilter ] = useState<Set<string>>(new Set())
  const [ sortingState, setSortingState ] = useState<ISortingState>({
    activeSort: "name",
    possibleSort: [ "name", "unit", "price/unit", "energy/unit", "protein/unit" ],
    activeOrder: "ascending",
    possibleOrder: [ "ascending", "descending" ],
  })

  const ingredientsContext = useContext(DataContext).ingredients
  const ingredientsListToShow = (() => {
    if (categoryFilter.size === 0)
      return ingredientsContext.ingredientsList.sort((a, b) => compareIngredients(
        a, b, sortingState.activeSort, sortingState.activeOrder === "ascending"
      ))

    return ingredientsContext.ingredientsList.filter(ingredient => {
      for (let index = 0; index < ingredient.inCategories.length; index++)
        if (categoryFilter.has(ingredient.inCategories[index]))
          return true

      return false
    }).sort((a, b) => compareIngredients(
      a, b, sortingState.activeSort, sortingState.activeOrder === "ascending"
    ))
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
          <div className='ingredients-view__main__list-wrapper__sort-options-wrapper'>
            <label htmlFor="sortDropDown">Sort by:</label>
            <div className="box dropdown-container">
              <p>{capitalize(sortingState.activeSort)}</p>
              <menu className="dropdown-menu">
                {sortingState.possibleSort.filter(option => option !== sortingState.activeSort).map(option => (
                  <div key={option} className="dropdown-menu__menu-item"
                    onClick={() => setSortingState((prev => {return {...prev, activeSort: option}}))}
                  >
                    {option}
                  </div>
                ))}
              </menu>
            </div>

            <label htmlFor="orderDropDown">Order:</label>
            <div className="box dropdown-container">
              <p>{capitalize(sortingState.activeOrder)}</p>
              <menu className="dropdown-menu">
                {sortingState.possibleOrder.filter(option => option !== sortingState.activeOrder).map(option => (
                  <div key={option} className="dropdown-menu__menu-item"
                    onClick={() => setSortingState((prev => {return {...prev, activeOrder: option}}))}
                  >
                    {option}
                  </div>
                ))}
              </menu>
            </div>
          </div>

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
      </Main>
    </>
  )
}