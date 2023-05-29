import { useContext, useEffect, useState } from 'react'
import { useOutletContext } from "react-router-dom"

import '../styles/views/ingredients.css'

import { Main } from "../components/Main"
import { Sidebar } from "../components/Sidebar"
import { DataContext } from '../util/context'
import { capitalize } from '../util/capitalize'
import { ingredientsSort } from '../util/sorting'
import { IIngredient, ISortingState } from '../util/interfaces'

export const IngredientsView = () => {
  const ingredientsContext = useContext(DataContext).ingredients

  const [ showSidebar, /* setShowSidebar, */ ] = useOutletContext() as [ boolean, /* React.Dispatch<React.SetStateAction<boolean>> */ ]
  const [ ingredientsToShow, setIngredientsToShow ] = useState<IIngredient[]>(ingredientsContext.ingredientsList)
  const [ categoryFilter, setCategoryFilter ] = useState<Set<string>>(new Set())
  const [ searchFilter, setSearchFilter ] = useState<string>('')
  const [ sortingState, setSortingState ] = useState<ISortingState>({
    activeSort: "name",
    possibleSort: [ "name", "unit", "price/unit", "energy/unit", "protein/unit", "last updated" ],
    activeOrder: "ascending",
    possibleOrder: [ "ascending", "descending" ],
  })

  useEffect(() => {
    const categoryFilteredIngredients = categoryFilter.size === 0
      ? ingredientsContext.ingredientsList
      : ingredientsContext.ingredientsList.filter(ingredient => {
          for (let index = 0; index < ingredient.inCategories.length; index++)
            if (categoryFilter.has(ingredient.inCategories[index]))
              return true
    
          return false
        })

    const searchFilteredIngreients = searchFilter.length === 0
      ? categoryFilteredIngredients
      : categoryFilteredIngredients.filter(ingredient => new RegExp(searchFilter, 'i').test(ingredient.name))
      
    const sortedIngredients = searchFilteredIngreients.sort(ingredientsSort(sortingState.activeSort, sortingState.activeOrder === "ascending"))

    setIngredientsToShow(sortedIngredients) // eslint-disable-next-line
  }, [categoryFilter, searchFilter])

  useEffect(() => {
    setIngredientsToShow(prev => prev.slice().sort(ingredientsSort(sortingState.activeSort, sortingState.activeOrder === "ascending")))
  }, [sortingState])

  return(
    <>
      <Sidebar showSidebar={showSidebar}>
        <flex-wrapper className="sidebar-ingredientsview__searchfield-wrapper">
          <form className='searchfield-wrapper__search-form' onSubmit={(event) => {
            event.preventDefault()
            setSearchFilter(String(new FormData(event.currentTarget).get('searchTerm')))
          }}>
            <input type="text" className="search-form__input-field" name='searchTerm' placeholder='...search'/>
            <button type='submit' className="search-form__button">serach icon</button>
          </form>

          {searchFilter.length > 0 && (
            <div className='--dev-border'
              onClick={() => setSearchFilter('')}
            >
              {`search: ${searchFilter}  x`}
            </div>
          )}
        </flex-wrapper>

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
                <th className='--grid-header'>{"last updated"}</th>
              </tr>
            </thead>

            <tbody>
              {ingredientsToShow.map(ingredient => (
                <tr key={ingredient.id}>
                  <td className='--grid-entries' >{ingredient.name}</td>
                  <td className='--grid-entries' >{ingredient.unit}</td>
                  <td className='--grid-entries' >{ingredient.energyPerUnit}</td>
                  <td className='--grid-entries' >{ingredient.proteinPerUnit}</td>
                  <td className='--grid-entries' >{ingredient.pricePerUnit}</td>
                  <td className='--grid-entries' >{new Date(ingredient.lastUpdated).toISOString().split('T')[0]}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </Main>
    </>
  )
}