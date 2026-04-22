import { useEffect, useState } from "react";
import { ConfirmDialog } from "../components/ConfirmDialog";
import { TextFieldCustom } from "../components/TextFieldCustom";
import { apiClient } from "../clients/todo-api-utils";
import { TodoListUpdateDto } from "../clients/todo-api-client";

export const EditTodoListDialog = (props: {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
  todoListId: string;
  currentName: string;
}) => {
  // State
  const [name, setName] = useState<string>("");

  // Effects
  useEffect(() => {
    setName(props.currentName);
  }, [props.currentName]);

  // Utils
  const editTodoList = (id: string, newName: string) =>
    apiClient
      .todoListPUT(id, new TodoListUpdateDto({ name: newName }))
      .then(() => setName(""))
      .then(() => props.onSuccess());

  return (
    <ConfirmDialog
      title="Edit todo list"
      open={props.open}
      onClose={() => props.onClose()}
      onSuccess={() => editTodoList(props.todoListId, name)}
      successButtonText="Save"
      cancelButtonText="Cancel"
    >
      <TextFieldCustom
        label={"List name"}
        value={name}
        onChange={(v) => setName(v)}
        onEnter={() => {
          editTodoList(props.todoListId, name);
        }}
      />
    </ConfirmDialog>
  );
};
