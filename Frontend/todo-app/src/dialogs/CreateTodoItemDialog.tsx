import { useState } from "react";
import { apiClient } from "../clients/todo-api-utils";
import { TodoItemCreateDto } from "../clients/todo-api-client";
import { TextFieldCustom } from "../components/TextFieldCustom";
import { ConfirmDialog } from "../components/ConfirmDialog";

export const CreateTodoItemDialog = (props: {
  todoListId: string;
  open: boolean;
  onSubmit: () => void;
  onClose: () => void;
}) => {
  // Dialog state
  const [newTodoItemName, setNewTodoItemName] = useState<string>("");

  const addTodo = (todoListId: string, todoItemName: string) =>
    apiClient
      .todoItemPOST(
        new TodoItemCreateDto({
          todoListId: todoListId,
          title: todoItemName,
          checked: false,
        }),
      )
      .then(() => props.onSubmit());

  return (
    <ConfirmDialog
      open={props.open}
      title={"Add todo item"}
      successButtonText={"Add"}
      cancelButtonText={"Cancel"}
      onSuccess={() => {
        addTodo(props.todoListId, newTodoItemName);
        setNewTodoItemName("");
      }}
      onClose={() => {
        setNewTodoItemName("");
        props.onClose();
      }}
    >
      <TextFieldCustom
        label={"Description"}
        value={newTodoItemName}
        onChange={(v) => setNewTodoItemName(v)}
        onEnter={() => {
          addTodo(props.todoListId, newTodoItemName);
          setNewTodoItemName("");
        }}
      />
    </ConfirmDialog>
  );
};
