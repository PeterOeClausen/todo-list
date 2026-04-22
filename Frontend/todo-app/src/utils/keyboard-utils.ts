/** Util function for executing given func when Enter is pressed */
export const onEnter = (e: React.KeyboardEvent, func: () => void) => {
  if (e.key === "Enter") {
    func();
  }
};
