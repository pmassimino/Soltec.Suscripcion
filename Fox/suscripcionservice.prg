
PUBLIC pBaseUrl 
pBaseUrl = GetValue("SUSCRIPCION.URL")
IF EMPTY(pBaseUrl)
   pBaseURL = "https://dev.suscripcion.soltec-net.com.ar/api/"
   SetValue("SUSCRIPCION.URL",pBaseURL)
ENDIF
*Obtengos las claves para buscar los token en cache 
pClaveExpiredToken = GetClaveExpiredToken()
pClaveToken =GetClaveToken()
*Obtengo el token en cache
pExpiredToken = GetValue(pClaveExpiredToken)
pToken = GetValueM(pClaveToken)
*Reviso Vencimiento de Token
IF (!EMPTY(pExpiredToken))
   pExpiredTokenD = CTOT(pExpiredToken)
   IF DATETIME() > pExpiredTokenD
      pToken = ""  &&Si esta vencido elimino el token
   ENDIF   
ENDIF
*Si no hay token obtengo uno
IF EMPTY(pToken)      
   pUsuario = GetValue("SUSCRIPCION.USUARIO") 
   pPassword = GetValue("SUSCRIPCION.PASSWORD")
   pToken  = login(pUsuario,pPassword)
   pExpiredToken = ALLTRIM(TTOC(DATETIME() + (23*60*60)))
   SetValue(pClaveExpiredToken,pExpiredToken)
   SetValueM(pClaveToken,pToken)
ENDIF

pEstado = Estado(pToken)

IF pEstado="AVISO"
   DO FORM c:\proyectos\soltec.suscripcion\fox\frmavisodeuda.scx WITH pToken  
ENDIF
IF pEstado="SUSPENDIDO"
      DO FORM c:\proyectos\soltec.suscripcion\fox\frmsuspencion.scx WITH pToken      
ENDIF
RETURN pEstado

*Retorna la Clave de Token
FUNCTION GetClaveToken
pTerminal = substr(sys(0),1, ATC("#",sys(0))-2)
pUsuarioWin = substr(sys(0), ATC("#",sys(0))+2)
pUsu= "001"
pClaveUsu = pTerminal + "." + pUsuarioWin + "." + pUsu 
pClaveToken = "Token." + pClaveUsu  
RETURN pClaveToken
*Retorna la Clave de Expired Token
FUNCTION GetClaveExpiredToken
pTerminal = substr(sys(0),1, ATC("#",sys(0))-2)
pUsuarioWin = substr(sys(0), ATC("#",sys(0))+2)
pUsu= "001"
pClaveUsu = pTerminal + "." + pUsuarioWin + "." + pUsu 
pClaveExpiredToken = "ExpiredToken." + pClaveUsu   
RETURN pClaveExpiredToken
*Consulta el estado de la suscripcion
FUNCTION Estado
PARAMETERS pToken
pMethod = pBaseURL + "suscripcion/estado"
loHttp = CREATEOBJECT("WinHttp.WinHttpRequest.5.1")
loHttp.Open("GET", pMethod, .F.)  && El tercer par�metro .F. indica que la solicitud es as�ncrona
loHttp.setRequestHeader("Authorization", "Bearer " + pToken)  && Agrega el token al encabezado de autorizaci�n
pResult = ""
try
  loHttp.Send()
  presult = ""
  IF loHttp.status = 200
     lcJson = loHttp.ResponseText
     SET PROCEDURE TO c:\cereales\json.prg ADDITIVE  && Supongo que json.prg es un archivo que contiene funciones para manipular JSON
     loObj = json_decode(lcJson)
     presult = loObj._estado
  ENDIF
CATCH
    MESSAGEBOX("Error de comunicacion con servicio de suscripcion",64)   
ENDTRY

RETURN presult

*Login sistema de suscripcion
FUNCTION Login 
PARAMETERS pUsuario,pPass
LOCAL loHttp
loHttp = CREATEOBJECT("WinHttp.WinHttpRequest.5.1")
cData = '{"nombre":"' + pUsuario + '","password": "' + pPass + '"}' 
pMethod = pbaseURL + "login"
loHttp.Open("POST", pMethod, .f.)
loHttp.setRequestHeader("Content-Type", "application/json")  && Establece el tipo de contenido como JSON
TRY
loHttp.Send(cData)
IF loHttp.status = 200
   lcJson = loHttp.ResponseText
   SET PROCEDURE TO c:\cereales\json.prg ADDITIVE  && Supongo que json.prg es un archivo que contiene funciones para manipular JSON
   loObj = json_decode(lcJson)
   pToken = loObj._token
ENDIF
IF loHttp.status = 401
   lcJson = loHttp.ResponseText
   SET PROCEDURE TO c:\cereales\json.prg ADDITIVE  && Supongo que json.prg es un archivo que contiene funciones para manipular JSON
   loObj = json_decode(lcJson)
   pmessage = loObj.array(1).array(2)
   MESSAGEBOX("Validacion de usuario y contrase�a: " + pMessage,48)
ENDIF
CATCH
   MESSAGEBOX("Error de comunicacion con servicio de suscripcion",64)
   pToken = ""
ENDTRY

RETURN pToken

*Descarga el resumen de cuenta se la suscripcion 
FUNCTION DownloadResumenCta
PARAMETERS pToken 
* Crear un objeto WinHttpRequest
loRequest = CREATEOBJECT("WinHttp.WinHttpRequest.5.1")

* Establecer la URL de la API REST y el m�todo HTTP
*pBaseURL ="https://dev.suscripcion.soltec-net.com.ar/api/" &&GetValue("Api.Global.Url")
lcUrl = pBaseUrl + "suscripcion/resumenCtaCte"
loRequest.Open("GET", lcUrl, .F.)

* Agregar el encabezado de autorizaci�n

loRequest.SetRequestHeader("Authorization", "Bearer " + pToken)

* Enviar la solicitud HTTP
loRequest.Send()

* Verificar el c�digo de respuesta
IF loRequest.Status = 200
    * Guardar el archivo PDF en el sistema de archivos local
    lcSavePath = "C:\sae\ResumenCtaCte.pdf"
    lnFileHandle = FCREATE(lcSavePath)
    IF lnFileHandle > 0
        lcResponseText = loRequest.ResponseBody
        FWRITE(lnFileHandle, lcResponseText)
        FCLOSE(lnFileHandle)
        
         * Abrir el archivo para mostrarlo utilizando ShellExecute
        DECLARE INTEGER ShellExecute IN shell32 ;
            INTEGER nHwnd, ;
            STRING cOperation, ;
            STRING cFile, ;
            STRING cParameters, ;
            STRING cDirectory, ;
            INTEGER nShowWindow
        ShellExecute(0, "open", lcSavePath, "", "", 1)
    ELSE
        ? "Error al crear el archivo"
    ENDIF
ELSE
    ? "Error de solicitud HTTP:", loRequest.Status
ENDIF

* Liberar el objeto WinHttpRequest
RELEASE loRequest

**Setting Functions
FUNCTION GetValue
PARAMETERS pid
OpenTableSetting()
result = ""
SELECT SettingGlobal
SEEK pid ORDER id
IF FOUND()
   result = ALLTRIM(value)
ENDIF
RETURN result

FUNCTION SetValue
PARAMETERS pid,pValue
OpenTableSetting()
SELECT SettingGlobal
SEEK pid ORDER id
IF NOT FOUND()
   APPEND BLANK	
ENDIF
replace id WITH pid, value WITH pValue

FUNCTION SetValueM
PARAMETERS pid,pValue
OpenTableSetting()
SELECT SettingGlobal
SEEK pid ORDER id
IF NOT FOUND()
   APPEND BLANK	
ENDIF
replace id WITH pid, valueM WITH pValue

FUNCTION GetValueM
PARAMETERS pid
OpenTableSetting()
result = ""
SELECT SettingGlobal
SEEK pid ORDER id
IF FOUND()
   result = ALLTRIM(valueM)
ENDIF
RETURN result
*Abre l tabla de setting Global
FUNCTION OpenTableSetting
IF !USED("traini")
    USE c:\sae\trayecto\traini IN 0
ENDIF
SELECT traini
GO top
pUnidad = ALLTRIM(cod)
pTrayecto = pUnidad + "\sae\empresas\SettingGlobal"
IF !USED("SettingGlobal")
   USE &pTrayecto IN 0
ENDIF



