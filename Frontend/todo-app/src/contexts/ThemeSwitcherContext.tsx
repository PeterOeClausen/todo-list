import React, { useState } from "react";
import { CssBaseline, ThemeProvider } from "@mui/material";
import { darkTheme, lightTheme } from "../themes/themes";

export const defaultThemeOption: ThemeOption = "dark";

/** Strings representing ThemeOptions */
export type ThemeOption = "dark" | "light";

/** Context value */
export interface ThemeSwitcherContextType {
  selectedThemeOption: ThemeOption;
  setTheme: (themeOption: ThemeOption) => void;
}

/** Context */
export const ThemeSwitcherContext =
  React.createContext<ThemeSwitcherContextType>({
    selectedThemeOption: "dark",
    setTheme: () => {},
  });

/** ThemingContext Provider than can wrap app with ability to toggle state */
export const ThemeSwitcherContextProvider = (
  props: React.PropsWithChildren
) => {
  const [selectedThemeOption, setThemeOption] =
    useState<ThemeOption>(defaultThemeOption);

  return (
    <ThemeSwitcherContext.Provider
      value={{
        selectedThemeOption: selectedThemeOption,
        setTheme: (themeOption) => setThemeOption(themeOption),
      }}
    >
      <ThemeProvider
        theme={selectedThemeOption === "light" ? lightTheme : darkTheme}
      >
        <CssBaseline />
        {props.children}
      </ThemeProvider>
    </ThemeSwitcherContext.Provider>
  );
};
