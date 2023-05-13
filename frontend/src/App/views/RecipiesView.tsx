// import React, { useState, useEffect } from 'react';
// import axios from 'axios';

// import { IRecipy, IIngredient } from '../util/interfaces';

// let firstRender = true;
// const dev_url = "http://localhost:5001/api/Recipies/blodpudding%20med%20ägg%20och%20bacon"

export const RecipiesView = () => {
  // const [ recipyState, setRecipyState ] = useState<IRecipy>({} as IRecipy)
  // const [ isLoading, setIsLoading ] = useState<boolean>(true)

  // useEffect(() => {
  //   if (firstRender) {
  //     firstRender = false
  
  //     axios.get(dev_url).then(res => {
  //       setIsLoading(false)
  //       setRecipyState(res.data)
  //     }).catch(e => console.log(e.message))
  //   }
  // }, [])

  return (
    <>
      {/* {recipyState && !isLoading && (
        <>
          <h4>{recipyState.name}</h4>
          <p>{recipyState.description}</p>
          <h5>Inredients:</h5>
          <ul>
            {recipyState.ingredients.map(ingredient => (<li key={ingredient.name}>{`${ingredient.name}: ${ingredient.quantity} ${ingredient.unit}`}</li>))}
          </ul>
        </>
      )} */}
    </>
  )
}