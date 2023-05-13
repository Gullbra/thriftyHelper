import './styles/base.css';
import './styles/color-schema.css'
import './styles/responsivness.css'

import { Routing } from './Routing'
// import { useState, useEffect } from 'react';
// import { DataContext, IDataContext } from './util/context';
// import { fetching } from './util/fetching';
// import { IIngredient, IRecipy } from './util/interfaces';

// let firstRender = true
export const App = () => {
  // const [ data, setData ] = useState<IDataContext>({} as IDataContext)

  // useEffect (() => {
  //   if (firstRender === true) {
  //     firstRender = false

  //     const ingredientsPromise = fetching('Ingredients') as Promise<IIngredient[]>
  //     const recipiesPromise = fetching('Recipies') as Promise<IRecipy[]>

  //     Promise
  //       .all([ingredientsPromise, recipiesPromise])
  //       .then(res => {
  //         setData({
  //           ingredientsList: res[0], 
  //           recipiesList: res[1]
  //         })
  //         console.log('fetch successfull!')
  //       }) 
  //   }
  // })

  // console.log({data})

  return(
      <Routing/>
    // <DataContext.Provider value={data}>
    // </DataContext.Provider>
  )
}
