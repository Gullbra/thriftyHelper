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
    errorElement: <Layout><p style={{fontSize: "1.5rem", marginBottom: "2rem"}}>Ops! Nothing here...</p><p>404: Not found</p></Layout>
  }]

  return (<RouterProvider router={createBrowserRouter(wrappedRoutes)} />)
}