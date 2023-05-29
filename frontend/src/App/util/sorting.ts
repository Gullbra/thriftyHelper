import { IIngredient } from "./interfaces"

export const ingredientsSort = (compareBy: string, orderAscending: boolean): (ingredientA: IIngredient, ingredientB: IIngredient) => number => {
  return (ingredientA, ingredientB) => {
    const compareValues: number[] | string [] = (() => {
      switch (compareBy) {
        case "name":
          return [ingredientA.name, ingredientB.name]
        case "unit":
          return [ingredientA.unit, ingredientB.unit]
        case "price/unit":
          return [ingredientA.pricePerUnit, ingredientB.pricePerUnit]
        case "energy/unit":
          return [ingredientA.energyPerUnit, ingredientB.energyPerUnit]
        case "protein/unit":
          return [ingredientA.proteinPerUnit, ingredientB.proteinPerUnit]
        case "last updated":
          return [new Date(ingredientA.lastUpdated).getTime(), new Date(ingredientB.lastUpdated).getTime()]
        default:
          return []
      }
    }) ()
    
    if (compareValues[0] < compareValues[1]) 
      return orderAscending ? -1 : 1;
    
    if (compareValues[0] > compareValues[1])
      return orderAscending ? 1 : -1;
  
    return 0
  }
}