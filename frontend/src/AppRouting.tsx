import { 
  createBrowserRouter, 
  RouterProvider
} from "react-router-dom"
import type { RouteObject } from "react-router-dom"

import { Layout } from "./AppLayout"
import { Home } from "./views/Home"
import { IngredientsListView } from "./views/IngredientsListView"
import { RecipyView } from "./views/RecipyView"

export const Routing = () => {
  const viewRoutes: RouteObject[] = [
    {
      path: "/test",
      element: <div>Testing new react-router-feature</div>
    },
    {
      path: "/",
      element: <Home />
    },
    {
      path: "/ingredients",
      element: <IngredientsListView/>
    },
    {
      path: "/recipies",
      element: <RecipyView />
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