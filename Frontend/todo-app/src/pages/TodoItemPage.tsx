import {
  Button,
  Checkbox,
  Divider,
  IconButton,
  List,
  ListItem,
  Paper,
  Typography,
} from "@mui/material";
import { PageContainer } from "../components/PageContainer";
import { TodoItemUpdateDto, TodoListDto } from "../clients/todo-api-client";
import React, { useEffect, useState } from "react";
import { apiClient } from "../clients/todo-api-utils";
import { useNavigate, useParams } from "react-router-dom";
import { Add, ChevronLeft, Delete, Edit } from "@mui/icons-material";
import { onEnter } from "../utils/keyboard-utils";
import { preferredWidth } from "../styling/common-styling";
import { CreateTodoItemDialog } from "../dialogs/CreateTodoItemDialog";
import { ConfirmDialog } from "../components/ConfirmDialog";
import { TextFieldCustom } from "../components/TextFieldCustom";

export const TodoItemPage = () => {
  // State
  const { id } = useParams();
  const [todoList, setTodoList] = useState<TodoListDto>();
  const [selectedTodoItemId, setSelectedTodoItemId] = useState<string>("");

  // Dialog state
  const [newDialogOpen, setNewDialogOpen] = useState<boolean>(false);

  const [editDialogOpen, setEditDialogOpen] = useState<boolean>(false);
  const [editTodoItemName, setEditTodoItemName] = useState<string>("");
  const navigate = useNavigate();

  // Effects
  useEffect(() => {
    getTodoList(id!);
  }, [id]);

  // Http calls
  const getTodoList = (id: string) =>
    apiClient.todoListGET(id!).then((x) => setTodoList(x));

  const deleteTodoItem = (todoItemId: string) =>
    apiClient.todoItemDELETE(todoItemId).then(() => getTodoList(id!));

  const updateTodoItem = (
    todoItemId: string,
    todoItemUpdate: TodoItemUpdateDto,
  ) =>
    apiClient
      .todoItemPUT(todoItemId, todoItemUpdate)
      .then(() => getTodoList(id!));

  return (
    <PageContainer
      style={{
        display: "flex",
        justifyContent: "center",
      }}
    >
      <div style={{ width: preferredWidth }}>
        <div style={{ display: "flex", alignItems: "center" }}>
          <IconButton size="small" onClick={() => navigate("/..")}>
            <ChevronLeft sx={{ fontSize: "40px" }} />
          </IconButton>
          <Typography variant="h3">{todoList?.name}</Typography>
        </div>
        {todoList?.todoItems?.length === 0 ? (
          <></>
        ) : (
          <Paper style={{ marginTop: "1rem" }}>
            <List>
              {todoList?.todoItems?.map((todoItem, i) => (
                <React.Fragment key={todoItem.id}>
                  <ListItem
                    style={{ display: "flex", justifyContent: "space-between" }}
                  >
                    <div
                      style={{
                        display: "flex",
                        gap: "1rem",
                        alignItems: "center",
                      }}
                    >
                      <Checkbox
                        checked={todoItem.checked}
                        onChange={(e) =>
                          updateTodoItem(
                            todoItem.id,
                            new TodoItemUpdateDto({
                              title: todoItem.title,
                              checked: !todoItem.checked,
                              todoListId: todoItem.todoListId,
                            }),
                          )
                        }
                        onKeyDown={(e) =>
                          onEnter(e, () =>
                            updateTodoItem(
                              todoItem.id,
                              new TodoItemUpdateDto({
                                title: todoItem.title,
                                checked: !todoItem.checked,
                                todoListId: todoItem.todoListId,
                              }),
                            ),
                          )
                        }
                      />
                      <Typography>{todoItem.title}</Typography>
                    </div>
                    <div>
                      <IconButton
                        size="medium"
                        onClick={() => {
                          setSelectedTodoItemId(todoItem.id!);
                          setEditTodoItemName(todoItem.title!);
                          setEditDialogOpen(true);
                        }}
                      >
                        <Edit />
                      </IconButton>
                      <IconButton
                        size="medium"
                        onClick={() => deleteTodoItem(todoItem.id!)}
                      >
                        <Delete />
                      </IconButton>
                    </div>
                  </ListItem>
                  {i !== todoList!.todoItems!.length - 1 ? <Divider /> : <></>}
                </React.Fragment>
              ))}
            </List>
          </Paper>
        )}
        <div
          style={{
            marginTop: "1rem",
            display: "flex",
            justifyContent: "flex-end",
          }}
        >
          <Button
            variant="contained"
            endIcon={<Add />}
            onClick={() => setNewDialogOpen(true)}
          >
            Add todo
          </Button>
        </div>
        {/* Dialogs */}
        <CreateTodoItemDialog
          todoListId={id!}
          open={newDialogOpen}
          onSubmit={() => {
            setNewDialogOpen(false);
            getTodoList(id!);
          }}
          onClose={() => {
            setNewDialogOpen(false);
          }}
        />
        <ConfirmDialog
          open={editDialogOpen}
          title={"Edit todo"}
          successButtonText={"Save"}
          cancelButtonText={"Cancel"}
          onSuccess={() => {
            updateTodoItem(
              selectedTodoItemId,
              new TodoItemUpdateDto({
                title: editTodoItemName,
                checked: todoList?.todoItems?.find(
                  (x) => x.id === selectedTodoItemId,
                )?.checked!,
                todoListId: id!,
              }),
            );
            setEditDialogOpen(false);
            setEditTodoItemName("");
          }}
          onClose={() => {
            setEditDialogOpen(false);
            setEditTodoItemName("");
          }}
        >
          <TextFieldCustom
            label={"Description"}
            value={editTodoItemName}
            onChange={(v) => setEditTodoItemName(v)}
            onEnter={() => {
              updateTodoItem(
                selectedTodoItemId,
                new TodoItemUpdateDto({
                  title: editTodoItemName,
                  checked: todoList?.todoItems?.find(
                    (x) => x.id === selectedTodoItemId,
                  )?.checked!,
                  todoListId: id!,
                }),
              );
              setEditDialogOpen(false);
              setEditTodoItemName("");
            }}
          />
        </ConfirmDialog>
      </div>
    </PageContainer>
  );
};
