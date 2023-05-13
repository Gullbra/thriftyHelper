import { useState, useEffect } from 'react';

import './styles/base.css';
import './styles/color-schema.css'
import './styles/responsivness.css'

import { Routing } from './Routing'
import { DataContext, IDataContext } from './util/context';
import { fetching } from './util/fetching';
import { IIngredientsContextData, IRecipiesContextData } from './util/interfaces';

let firstRender = true
export const App = () => {
  const [ data, setData ] = useState<IDataContext>({} as IDataContext)

  useEffect (() => {
    if (firstRender === true) {
      firstRender = false

      const ingredientsPromise = fetching('Ingredients') as Promise<IIngredientsContextData>
      const recipiesPromise = fetching('Recipies') as Promise<IRecipiesContextData>
      
      Promise
        .all([ingredientsPromise, recipiesPromise])
        .then(res => setData({
          ingredients: res[0], 
          recipies: res[1]
        }))
        .finally(() => console.log('📮 fetching called!'))
    }
  })
  console.log({data})

  return(
    <DataContext.Provider value={data}>
      <Routing/>
    </DataContext.Provider>
  )
}
