import { TodoApiClient } from "./todo-api-client";

const baseUrl = "https://localhost:5000";

export const apiClient = new TodoApiClient(baseUrl);
