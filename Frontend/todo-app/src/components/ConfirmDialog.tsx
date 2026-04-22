import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
} from "@mui/material";

interface ConfirmDialogProps {
  open: boolean;
  title: string;
  successButtonText: string;
  cancelButtonText: string;
  onSuccess: () => void;
  onClose: () => void;
}

export const ConfirmDialog = (
  props: React.PropsWithChildren<ConfirmDialogProps>
) => {
  return (
    <Dialog maxWidth="sm" open={props.open} onClose={() => props.onClose()}>
      <DialogTitle id="responsive-dialog-title">{props.title}</DialogTitle>
      <DialogContent>{props.children}</DialogContent>
      <DialogActions>
        <Button
          variant="contained"
          color="primary"
          onClick={() => props.onSuccess()}
        >
          {props.successButtonText}
        </Button>
        <Button
          variant="contained"
          color="secondary"
          onClick={() => props.onClose()}
        >
          {props.cancelButtonText}
        </Button>
      </DialogActions>
    </Dialog>
  );
};
