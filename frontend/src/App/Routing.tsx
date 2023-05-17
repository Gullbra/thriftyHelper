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

  // TODO: protected user routes with some kind of jwt
  const authRoutes: RouteObject[] = []

  const wrappedRoutes: RouteObject[] = [{
    path: "/",
    element: <Layout />,
    children: [...viewRoutes, ...authRoutes],
    errorElement: <Layout>404: Not found</Layout>
  }]

  return (<RouterProvider router={createBrowserRouter(wrappedRoutes)} />)
}