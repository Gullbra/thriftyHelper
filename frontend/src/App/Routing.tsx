import { 
  createBrowserRouter, 
  RouterProvider
} from "react-router-dom"
import type { RouteObject } from "react-router-dom"

import { Layout } from "./Layout"
import { Home } from "./views/Home"
import { IngredientsView } from "./views/IngredientsView"
import { RecipiesView } from "./views/RecipiesView"

export const Routing = () => {
  const viewRoutes: RouteObject[] = [
    {
      path: "/",
      element: <Home/>
    },
    {
      path: "/ingredients",
      element: <IngredientsView/>
    },
    {
      path: "/recipies",
      element: <RecipiesView/>
    },
    {
      path: "/mealplaner",
      element: <>Not yet Implemented</>
    }
  ]

  // //const loading
 
  const authRoutes: RouteObject[] = []

  const wrappingRoutes: RouteObject[] = [{
    path: "/",
    element: <Layout><></></Layout>,
    children: [...viewRoutes, ...authRoutes],
    errorElement: <Layout>404: Not found</Layout>
  }]

  // const root: RouteObject[] = [{
  //   path: "/",
  //   element: <Layout />,
  //   //children: [...viewRoutes, ...authRoutes],
  //   errorElement: <Layout><>Hey</></Layout>
  // }]

  return (<RouterProvider router={createBrowserRouter(
    wrappingRoutes
  )} />)
}