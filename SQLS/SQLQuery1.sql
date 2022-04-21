-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TIPOS_CUENTAS_INSERTAR
	-- Add the parameters for the stored procedure here
	@NOMBRE nvarchar(50),
	@ID_USUARIO int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @ORDEN int;
	SELECT @ORDEN = COALESCE(MAX(ORDEN),0) + 1
	FROM TIPOS_CUENTAS
	WHERE ID_USUARIO = @ID_USUARIO

	INSERT INTO TIPOS_CUENTAS (NOMBRE, ID_USUARIO, ORDEN) VALUES (@NOMBRE, @ID_USUARIO, @ORDEN);

END
GO
