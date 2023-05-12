import { 
  createBrowserRouter, 
  RouterProvider
} from "react-router-dom"
import type { RouteObject } from "react-router-dom"

import { Home } from "./views/Home"
import { Layout } from "./AppLayout"

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
  ]

  // const authRoutes: RouteObject[] = []

  const wrappingRoutes: RouteObject[] = [{
    path: "/",
    element: <Layout />,
    children: viewRoutes
  }]

  return (<RouterProvider router={createBrowserRouter(wrappingRoutes)} />)
}