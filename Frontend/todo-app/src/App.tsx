import { RouterProvider } from "react-router-dom";
import { router } from "./routing/router";
import { ThemeSwitcherContextProvider } from "./contexts/ThemeSwitcherContext";

const App = () => {
  return (
    <ThemeSwitcherContextProvider>
      <RouterProvider router={router} />
    </ThemeSwitcherContextProvider>
  );
};

export default App;
