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
      path: "/test",
      element: <>Testing new react-router-feature</>
    },
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
      element: <RecipiesView />
    },
    {
      path: "/mealplaner",
      element: <>Not yet Implemented</>
    }
  ]
 
  // ! should be dynamic depending on if a user is logged in or not
  const authRoutes: RouteObject[] = []

  const wrappingRoutes: RouteObject[] = [{
    path: "/",
    element: <Layout />,
    children: [...viewRoutes, ...authRoutes]
  }]

  return (<RouterProvider router={createBrowserRouter(wrappingRoutes)} />)
}