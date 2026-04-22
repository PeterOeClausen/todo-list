import { TextField } from "@mui/material";
import { onEnter } from "../utils/keyboard-utils";

interface TextFieldCustomProps {
  label: string;
  value: string;
  onChange: (value: string) => void;
  onEnter: () => void;
}

export const TextFieldCustom = (props: TextFieldCustomProps) => {
  return (
    <TextField
      sx={{ width: '300px' }}
      variant={"standard"}
      label={props.label}
      focused
      value={props.value}
      onChange={(e) => props.onChange(e.target.value)}
      onKeyDown={(e) =>
        onEnter(e, () => {
          e.preventDefault();
          props.onEnter();
        })
      }
    />
  );
};
