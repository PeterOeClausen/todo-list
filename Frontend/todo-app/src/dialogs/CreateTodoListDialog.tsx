import { useState } from "react";
import { ConfirmDialog } from "../components/ConfirmDialog";
import { TextFieldCustom } from "../components/TextFieldCustom";
import { apiClient } from "../clients/todo-api-utils";
import { TodoListCreateDto } from "../clients/todo-api-client";

export const CreateTodoListDialog = (props: {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}) => {
  const [newName, setNewName] = useState<string>("");
  const createTodoList = (name: string) =>
    apiClient
      .todoListPOST(new TodoListCreateDto({ name: name! }))
      .then(() => setNewName(""))
      .then(() => props.onSuccess());

  return (
    <ConfirmDialog
      title="Create new todo list"
      open={props.open}
      onClose={() => props.onClose()}
      onSuccess={() => createTodoList(newName)}
      successButtonText="Create"
      cancelButtonText="Cancel"
    >
      <TextFieldCustom
        label={"List name"}
        value={newName}
        onChange={(v) => setNewName(v)}
        onEnter={() => createTodoList(newName)}
      />
    </ConfirmDialog>
  );
};
