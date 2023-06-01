import { useContext, useEffect, useState } from 'react'
import { useOutletContext } from "react-router-dom"

import '../styles/views/ingredients.css'

import { Main } from "../components/Main"
import { Sidebar } from "../components/Sidebar"
import { DataContext } from '../util/context'
import { capitalize } from '../util/capitalize'
import { ingredientsSort } from '../util/sorting'
import { IIngredient, ISortingState } from '../util/interfaces'

interface IPaginationState {
  currentPage: number,
  maxPage: number,
  pageLimit: number,
  possibleLimits: number[]
}

export const IngredientsView = () => {
  const ingredientsContext = useContext(DataContext).ingredients

  const [ showSidebar, /* setShowSidebar, */ ] = useOutletContext() as [ boolean, /* React.Dispatch<React.SetStateAction<boolean>> */ ]
  const [ ingredientsToShow, setIngredientsToShow ] = useState<IIngredient[]>(ingredientsContext.ingredientsList)
  const [ paginationState, setPaginationState ] = useState<IPaginationState>({ currentPage: 1, maxPage: Math.ceil(ingredientsToShow.length / 15), pageLimit: 15, possibleLimits: [15, 20, 25, 50, 100] })
  const [ currentPageList, setCurrentPageList ] = useState<IIngredient[]>(ingredientsContext.ingredientsList.slice().splice(0, paginationState.pageLimit-1))
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


  useEffect(() => {
    const newMaxpage = Math.ceil(ingredientsToShow.length / paginationState.pageLimit)

    if (paginationState.currentPage > newMaxpage) {
      setPaginationState(prev => {return{
        ...prev, 
        currentPage: 1, 
        maxPage: newMaxpage 
      }})
    } else if (newMaxpage !== paginationState.maxPage) {
      setPaginationState(prev => {return{
        ...prev,
        maxPage: newMaxpage 
      }})
    } // eslint-disable-next-line
  }, [ingredientsToShow, paginationState.pageLimit])


  useEffect(() => {
    setCurrentPageList(ingredientsToShow.slice().splice((paginationState.currentPage-1) * paginationState.pageLimit, paginationState.currentPage * paginationState.pageLimit))
  }, [paginationState, ingredientsToShow])


  return(
    <>
      <Sidebar showSidebar={showSidebar}>
        <SidebarSearch searchFilter={searchFilter} setSearchFilter={setSearchFilter} />
        <CategoriesList categoriesArray={ingredientsContext.categories} categoryFilter={categoryFilter} setCategoryFilter={setCategoryFilter} />
      </Sidebar>

      <Main showSidebar={showSidebar}>
        <ModeOptions />

        <div className="ingredients-view__main__list-wrapper">
          <Optionsbar currentPageList={currentPageList} ingredientsToShow={ingredientsToShow} paginationState={paginationState} setPaginationState={setPaginationState} sortingState={sortingState} setSortingState={setSortingState}/>

          <TableAndPaginationButtons currentPageList={currentPageList} setSortingState={setSortingState} paginationState={paginationState} setPaginationState={setPaginationState}/>
        </div>
      </Main>
    </>
  )
}

const SidebarSearch = ({searchFilter, setSearchFilter}: {searchFilter: string, setSearchFilter: React.Dispatch<React.SetStateAction<string>>}) => (
  <flex-wrapper className="sidebar-ingredientsview__searchfield-wrapper">
    <form className='searchfield-wrapper__search-form' onSubmit={(event) => {
      event.preventDefault()
      setSearchFilter(String(new FormData(event.currentTarget).get('searchTerm')))
    }}>
      <input type="text" className="search-form__input-field" name='searchTerm' placeholder='...search'/>
      <button type='submit' className="search-form__button">search icon</button>
    </form>

    {searchFilter.length > 0 && (
      <div className='--dev-border'
        onClick={() => setSearchFilter('')}
      >
        {`search: ${searchFilter}  x`}
      </div>
    )}
  </flex-wrapper>
)

const ModeOptions = () => (
  <flex-wrapper class='ingredient-view__main__mode-choice-container'>
    <h4>Ingredients List</h4>
    {/* //! Auth protected? */}
    <h4>Add new Ingredient</h4>
  </flex-wrapper>
)

const CategoriesList = ({categoriesArray, categoryFilter, setCategoryFilter}: {categoriesArray: string[], categoryFilter: Set<string>, setCategoryFilter: React.Dispatch<React.SetStateAction<Set<string>>>}) => (
  <>
    {categoriesArray.map(category => (
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
  </>
)

const Optionsbar = (
  {currentPageList, ingredientsToShow, paginationState, setPaginationState, sortingState, setSortingState}:
  {currentPageList: IIngredient[], ingredientsToShow: IIngredient[], paginationState: IPaginationState, setPaginationState: React.Dispatch<React.SetStateAction<IPaginationState>>, sortingState: ISortingState, setSortingState: React.Dispatch<React.SetStateAction<ISortingState>>}
) => (
  <div className="list-wrapper__options-bar">
    <div className="box dropdown-container">
      <p>{`Showing ${currentPageList.length} of ${ingredientsToShow.length} ingredients`}</p>
      <menu className="dropdown-menu">
        <div>
          <label htmlFor="setPageLimit">set pagelimit: </label>
          {/** change to this?: https://www.w3schools.com/tags/att_input_list.asp */}

          {paginationState.possibleLimits.filter(limit => limit !== paginationState.pageLimit).map(limit => (
            <div key={"pageLimOp" + limit} className="dropdown-menu__menu-item"
              onClick={() => setPaginationState((prev => {return {...prev, pageLimit: limit}}))}
            >
              {limit}
            </div>
          ))}
        </div>
      </menu>
    </div>
    
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
  </div>
)

const TableAndPaginationButtons = (
  {currentPageList, setSortingState, paginationState, setPaginationState}:
  {currentPageList: IIngredient[], setSortingState: React.Dispatch<React.SetStateAction<ISortingState>>, paginationState: IPaginationState, setPaginationState: React.Dispatch<React.SetStateAction<IPaginationState>>}
) => (
  <>
    <table className="list-wrapper__table-element --dev-border">
      <thead>
        <tr>
          {['name', "unit", "energy/unit", "protein/unit", "price/unit", "last updated"].map(colHeader => (
            <th className='--grid-header'
              key={`col-header ${colHeader}`}
              onClick={() => setSortingState((prev => {return {...prev, activeSort: colHeader}}))}
            >
              {colHeader}
            </th>
          ))}
        </tr>
      </thead>

      <tbody>
        {currentPageList.map(ingredient => (
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

    <div className="list-wrapper__pagination-wrapper">
      {paginationState.currentPage > 1 && (
        <>
          <span onClick={() => setPaginationState(prev => {return {...prev, currentPage: prev.currentPage - 1}})}>{"<"}</span>
          <span>...</span>
        </>
      )}
      <span>{paginationState.currentPage}</span>
      {paginationState.currentPage < paginationState.maxPage && (
        <> 
          <span>...</span>
          <span onClick={() => setPaginationState(prev => {return {...prev, currentPage: prev.currentPage + 1}})}>{">"}</span>
        </>
      )}
    </div>
  </>
)