import { createContext } from "react"
import { IIngredient, IRecipy } from "./interfaces";

export interface IDataContext { 
  ingredientsList: IIngredient[],
  recipiesList: IRecipy[]
}
export const DataContext = createContext<IDataContext>({ingredientsList: [], recipiesList: []})