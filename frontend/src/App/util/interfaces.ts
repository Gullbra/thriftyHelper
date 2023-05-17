export interface IIngredient {
  id: number,
  name: string,
  unit: string,
  pricePerUnit: number,
  energyPerUnit: number,
  proteinPerUnit: number,
  lastUpdated: string,
  inCategories: string[]
}

export interface IRecipy {
  id: number,
  name: string,
  description: string,
  ingredients: {
    id: number,
    quantity: number
  } [],
  inCategories: string[]
}

export interface IIngredientsContextData {
  ingredientsList: IIngredient[],
  categories: string[]
}

export interface IRecipiesContextData {
  recipiesList: IRecipy[],
  categories: string[]
}
