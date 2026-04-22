import { useContext } from "react";
import { ThemeSwitcherContext } from "../contexts/ThemeSwitcherContext";
import { IconButton } from "@mui/material";
import { DarkMode, LightMode } from "@mui/icons-material";

export const ThemeToggleButton = () => {
  const { selectedThemeOption, setTheme } = useContext(ThemeSwitcherContext);

  return (
    <IconButton
      onClick={() =>
        selectedThemeOption === "dark" ? setTheme("light") : setTheme("dark")
      }
    >
      {selectedThemeOption === "dark" ? <LightMode /> : <DarkMode />}
    </IconButton>
  );
};
