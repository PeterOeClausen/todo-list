import React from "react";
import { useEffect, useState } from "react";
import { TodoListDto } from "../clients/todo-api-client";
import { apiClient } from "../clients/todo-api-utils";
import {
  Button,
  Divider,
  IconButton,
  List,
  ListItem,
  Paper,
  Typography,
} from "@mui/material";
import { Add, Delete, Edit } from "@mui/icons-material";
import { PageContainer } from "../components/PageContainer";
import { useNavigate } from "react-router-dom";
import ChecklistIcon from "@mui/icons-material/Checklist";
import { onEnter } from "../utils/keyboard-utils";
import { ThemeToggleButton } from "../components/ThemeToggleButton";
import { preferredWidth } from "../styling/common-styling";
import { CreateTodoListDialog } from "../dialogs/CreateTodoListDialog";
import { EditTodoListDialog } from "../dialogs/EditTodoListDialog";

export const TodoListPage = () => {
  const navigate = useNavigate();

  // State
  const [todoLists, setTodoLists] = useState<TodoListDto[]>([]);

  const [newDialogOpen, setNewDialogOpen] = useState<boolean>(false);

  const [editDialogOpen, setEditDialogOpen] = useState<boolean>(false);
  const [editName, setEditName] = useState<string>("");
  const [editId, setEditId] = useState<string>("");

  // Effects
  useEffect(() => {
    getTodoLists();
  }, []);

  // Http calls
  const getTodoLists = () =>
    apiClient.todoListAll().then((x) => setTodoLists(x));

  const deleteTodoList = (id: string) =>
    apiClient.todoListDELETE(id).then(() => getTodoLists());

  return (
    <PageContainer
      style={{
        display: "flex",
        justifyContent: "center",
      }}
    >
      <div style={{ width: preferredWidth }}>
        <div style={{ display: "flex", justifyContent: "space-between" }}>
          <Typography variant="h3">{"Todo lists"}</Typography>
          <ThemeToggleButton />
        </div>
        <Paper style={{ marginTop: "1rem", minWidth: "300px" }}>
          <List>
            {todoLists.map((x, i) => (
              <React.Fragment key={x.id!}>
                <ListItem
                  style={{
                    display: "flex",
                    justifyContent: "space-between",
                    gap: "1rem",
                  }}
                >
                  <div
                    onClick={() => navigate(`/todo-lists/${x.id!}`)}
                    tabIndex={0}
                    style={{ cursor: "pointer", display: "flex", gap: "1rem" }}
                    onKeyDown={(e) =>
                      onEnter(e, () => navigate(`/todo-lists/${x.id!}`))
                    }
                  >
                    <ChecklistIcon />
                    <Typography>{x.name}</Typography>
                  </div>
                  <div>
                    <IconButton
                      size="medium"
                      onClick={() => {
                        setEditId(x.id!);
                        setEditName(x.name!);
                        setEditDialogOpen(true);
                      }}
                    >
                      <Edit />
                    </IconButton>
                    <IconButton
                      size="medium"
                      onClick={() => deleteTodoList(x.id!)}
                    >
                      <Delete />
                    </IconButton>
                  </div>
                </ListItem>
                {i !== todoLists.length - 1 ? <Divider /> : <></>}
              </React.Fragment>
            ))}
          </List>
        </Paper>
        <div
          style={{
            display: "flex",
            justifyContent: "flex-end",
            paddingTop: "1rem",
            gap: "1rem",
          }}
        >
          <Button
            endIcon={<Add />}
            variant="contained"
            onClick={() => setNewDialogOpen(true)}
          >
            Create
          </Button>
        </div>
        {/* Dialogs */}
        <CreateTodoListDialog
          open={newDialogOpen}
          onClose={() => {
            setNewDialogOpen(false);
          }}
          onSuccess={() => {
            getTodoLists();
            setNewDialogOpen(false);
          }}
        />
        <EditTodoListDialog
          open={editDialogOpen}
          onClose={() => setEditDialogOpen(false)}
          onSuccess={() => {
            setEditDialogOpen(false);
            getTodoLists();
          }}
          todoListId={editId}
          currentName={editName}
        />
      </div>
    </PageContainer>
  );
};
