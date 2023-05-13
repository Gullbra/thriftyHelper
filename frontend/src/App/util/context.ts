import { createContext } from "react"
import { IIngredientsContextData, IRecipiesContextData } from "./interfaces";

export interface IDataContext { 
  ingredients: IIngredientsContextData,
  recipies: IRecipiesContextData
}
export const DataContext = createContext<IDataContext>({} as IDataContext)