import { Button, Typography } from "@mui/material";
import { PageContainer } from "../components/PageContainer";
import { useNavigate } from "react-router-dom";
import { preferredWidth } from "../styling/common-styling";

export const PageNotFoundPage = () => {
  const navigate = useNavigate();
  return (
    <PageContainer style={{ display: "flex", justifyContent: "center" }}>
      <div style={{ width: preferredWidth }}>
        <Typography variant="h3">404: Page not found</Typography>
        <Typography variant="body1">This page doesn't exists.</Typography>
        <div style={{ display: "flex", justifyContent: "center" }}>
          <Button
            style={{ marginTop: "1rem" }}
            variant="contained"
            color="primary"
            onClick={() => navigate("/")}
          >
            Return home
          </Button>
        </div>
      </div>
    </PageContainer>
  );
};
