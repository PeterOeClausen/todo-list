import { createBrowserRouter, Navigate } from "react-router-dom";
import { TodoListPage } from "../pages/TodoListPage";
import { TodoItemPage } from "../pages/TodoItemPage";
import { PageNotFoundPage } from "../pages/PageNotFoundPage";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <Navigate to={"/todo-lists"} />,
  },
  {
    path: "/todo-lists",
    element: <TodoListPage />,
  },
  {
    path: "/todo-lists/:id",
    element: <TodoItemPage />,
  },
  {
    path: "/*",
    element: <PageNotFoundPage />,
  },
]);
