export interface IIngredient {
  name: string,
  unit: string,
  quantity?: number,
  pricePerUnit: number,
  energyPerUnit: number,
  proteinPerUnit: number
}

export interface IRecipy {
  name: string,
  description: string,
  ingredients: IIngredient[]
}