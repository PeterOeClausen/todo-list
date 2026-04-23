# todo-app

The React frontend for the Todo List application. It talks to [`TodoList.Api`](../../TodoList.Api) through a TypeScript client generated from the API's OpenAPI spec, so the client is always in sync with the server contract.

Bootstrapped with [Create React App](https://github.com/facebook/create-react-app) (TypeScript template).

## Tech stack

- **React 19** + **TypeScript** on Create React App 5.
- **Material UI (MUI) 9** with Emotion for styling, plus `@mui/icons-material`.
- **React Router 6** (`createBrowserRouter` + `RouterProvider`) for routing.
- **NSwag-generated API client** in `src/clients/todo-api-client.ts` — do not edit by hand, regenerate via the solution's `Scripts/generate-ts-client.sh`.
- **Jest + React Testing Library** via `react-scripts test`.
- Light/dark theme switching via a custom `ThemeSwitcherContext`.

## Prerequisites

- Node.js 18+ and npm
- A running instance of [`TodoList.Api`](../../TodoList.Api) at <https://localhost:5000>. See that project's README for how to start the API and its SQL Server.

The API's CORS policy allows `http://localhost:3000`, so the dev server works out of the box against a local API.

## Project structure

```
src/
├── App.tsx                 Wraps RouterProvider in ThemeSwitcherContextProvider
├── index.tsx               Entry point
├── clients/
│   ├── todo-api-client.ts  NSwag-generated TypeScript client (do not hand-edit)
│   └── todo-api-utils.ts   Singleton apiClient pointing at https://localhost:5000
├── components/             Reusable UI: PageContainer, ConfirmDialog, TextFieldCustom, ThemeToggleButton
├── contexts/               ThemeSwitcherContext (light/dark toggle)
├── dialogs/                CreateTodoListDialog, EditTodoListDialog, CreateTodoItemDialog
├── pages/                  TodoListPage (all lists), TodoItemPage (items in a list), PageNotFoundPage
├── routing/router.tsx      Route table
├── styling/                Shared MUI style helpers
├── themes/themes.ts        MUI theme definitions
└── utils/                  Small utilities (e.g. keyboard-utils)
```

### Routes

| Path | Component | Purpose |
| --- | --- | --- |
| `/` | redirect | Navigates to `/todo-lists`. |
| `/todo-lists` | `TodoListPage` | Browse, create, edit, and delete todo lists. |
| `/todo-lists/:id` | `TodoItemPage` | View and manage items inside one list. |
| `/*` | `PageNotFoundPage` | 404 fallback. |

## API configuration

The API base URL is hard-coded in `src/clients/todo-api-utils.ts`:

```ts
const baseUrl = "https://localhost:5000";
export const apiClient = new TodoApiClient(baseUrl);
```

If you point the frontend at a different API (e.g. a deployed instance), update that constant. Because CRA bakes configuration at build time, you could also promote it to a `REACT_APP_*` env var and read it from `process.env` — kept simple here for showcase purposes.

## Regenerating the API client

When the API contract changes, regenerate `src/clients/todo-api-client.ts` from the API's OpenAPI spec using the helper script at the root of the repo:

```bash
# 1. Start TodoList.Api so /openapi/v1.json is reachable
# 2. Install NSwag CLI once:
dotnet tool install --global NSwag.ConsoleCore

# 3. Run the generator
cd ../../Scripts
./generate-ts-client.sh
```

The script fetches the spec and runs NSwag's `openapi2tsclient` to overwrite this project's client.

## Available scripts

Run these from `Frontend/todo-app`.

### `npm start`

Runs the app in development mode at <http://localhost:3000>. The page hot-reloads on save and surfaces lint errors in the console.

### `npm test`

Launches Jest in interactive watch mode. See the [Create React App testing docs](https://facebook.github.io/create-react-app/docs/running-tests) for more.

### `npm run build`

Builds the app for production into `build/`. Bundles are minified and filenames include content hashes.

### `npm run eject`

**One-way operation.** Copies CRA's build configuration into this project. Avoid unless there's a concrete need — the current feature set does not require it.

## Learn more

- [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started)
- [React documentation](https://react.dev/)
- [Material UI documentation](https://mui.com/material-ui/getting-started/)
- [React Router documentation](https://reactrouter.com/)
