import { Components, createTheme, Theme } from "@mui/material";

const componentSettings: Components<Omit<Theme, "components">> = {
  MuiButton: {
    defaultProps: {
      style: {
        textTransform: "none", // Do not capitalize buttons
      },
    },
  },
};

/** Dark theme settings */
export const darkTheme = createTheme({
  palette: {
    mode: "dark",
    primary: {
      main: "#BB20BC",
    },
    secondary: {
      main: "#4451DD",
    },
  },
  components: componentSettings,
});

/** Light theme settings */
export const lightTheme = createTheme({
  palette: {
    mode: "light",
  },
  components: componentSettings,
});
