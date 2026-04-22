import React from "react";

interface PageContainerProps {
  style?: React.CSSProperties;
}

export const PageContainer = (
  props: React.PropsWithChildren<PageContainerProps>
) => {
  return (
    <div style={{ padding: "1rem", ...props.style }}>{props.children}</div>
  );
};
