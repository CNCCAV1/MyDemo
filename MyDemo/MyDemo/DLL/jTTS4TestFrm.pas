unit jTTS4TestFrm;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, jTTS_ML;

type
  TfrmMain = class(TForm)
    Label1: TLabel;
    btnSpeak: TButton;
    btnPause: TButton;
    btnResume: TButton;
    btnStop: TButton;
    btnPlayToFile: TButton;
    btnSessionData: TButton;
    memContent: TMemo;
    cobVoice: TComboBox;
    Label2: TLabel;
    cobCodePage: TComboBox;
    Label3: TLabel;
    btnOpen: TButton;
    dlgOpenFile: TOpenDialog;
    procedure btnSpeakClick(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure FormCreate(Sender: TObject);
    procedure cobVoiceChange(Sender: TObject);
    procedure cobCodePageChange(Sender: TObject);
    procedure btnOpenClick(Sender: TObject);
    procedure btnPauseClick(Sender: TObject);
    procedure btnResumeClick(Sender: TObject);
    procedure btnStopClick(Sender: TObject);
    procedure btnPlayToFileClick(Sender: TObject);
    procedure MessageHandler(var Msg: TMsg; var Handled: Boolean);
    procedure btnSessionDataClick(Sender: TObject);

  private
    { Private declarations }
    procedure ErrorLog(LogFileName:string;ErrorMessage:string);
  public
    { Public declarations }
  end;

var
  frmMain: TfrmMain;
  gVoiceIDs:TStrings;
  gjttsConf:JTTS_CONFIG;
  gSessionID:DWORD;
  gLogfont:LOGFONT;
  gFileName:String;
  gCharset:Cardinal;

  gContent:string;

//播放通知回调函数
function  PlayToFileNotify(wParam:Word; lParam:LongInt; dwUserData:DWORD):BOOL; stdcall;
function LoadFileToAnsiString(filename:String; codePage:Integer):String;

implementation

{$R *.dfm}
procedure TfrmMain.ErrorLog(LogFileName:string;ErrorMessage:string);
var
  F1:TextFile;
begin
  if not FileExists(LogFileName) then
    begin
      AssignFile(F1, LogFileName);
      ReWrite(F1);
      Writeln(F1,FormatDateTime('YYYY"年"MM"月"DD"日" HH:MM:SS',Now));
      Writeln(F1,ErrorMessage);
      Writeln(F1);
      Flush(F1);
      CloseFile(F1);
    end
  else
    begin
      AssignFile(F1, LogFileName);
      Append(F1);
      Writeln(F1,FormatDateTime('YYYY"年"MM"月"DD"日" HH:MM:SS',Now));
      Writeln(F1,ErrorMessage);
      Writeln(F1);
      Flush(F1);
      CloseFile(F1);
    end;
end;

procedure TfrmMain.btnSpeakClick(Sender: TObject);
var
    err:ERRCODE;
begin
  try
    err:= jTTS_Play(PChar(gContent),PLAYCONTENT_TEXT or PLAY_INTERRUPT or PLAYMODE_ASYNC);
    if err<>ERR_NONE then
      ShowMessage('jTTS_Play出错:' + IntToStr(Integer(err)) ) ;
    //ErrorLog('error.txt',IntToStr(Integer(err)));
  except
    On E:Exception do
      begin
        ErrorLog('error.txt',E.Message);
      end;
  end;
  //Close;
end;

procedure TfrmMain.FormClose(Sender: TObject; var Action: TCloseAction);
begin
    jTTS_End();
end;

procedure TfrmMain.FormCreate(Sender: TObject);
var
    err:ERRCODE;
    voiceCount:Integer;
    i:Integer;
    voiceAttr:JTTS_VOICEATTRIBUTE;
    langAttr:JTTS_LANGATTRIBUTE;
    str:string;
begin

    err:= jTTS_Init(nil,'');
    if err<>ERR_NONE then begin
        ShowMessage('jTTS初始化错:' + IntToStr(Integer(err)));
        Exit;
    end;

    with gjttsConf do begin
        wVersion := JTTS_VERSION4;
        nCodePage:= CODEPAGE_GB;
        ZeroMemory(@szVoiceID, SizeOf(szVoiceID));
        nDomain:= DOMAIN_COMMON;
        nPitch := 5;
        nVolume:= 5;
        nSpeed := 0;
        nPuncMode := PUNC_OFF;
        nDigitMode:= DIGIT_AUTO;
        nEngMode:= ENG_AUTO;
        nTagMode:= TAG_AUTO;
        ZeroMemory(@nReserved, SizeOf(nReserved));
    end;

    gVoiceIDs:= TStringList.Create();

    voiceCount:= jTTS_GetVoiceCount();

    for i:=0 to voiceCount-1 do begin
        jTTS_GetVoiceAttribute(i, @voiceAttr);
        gVoiceIDs.Add(voiceAttr.szVoiceID);
        jTTS_GetLangAttributeByValue(voiceAttr.nLanguage, @langAttr);
        str:= voiceAttr.szName + '(' + langAttr.szName;

        if voiceAttr.nGender = GENDER_FEMALE then
            str := str + ' 女声'
        else if voiceAttr.nGender = GENDER_MALE then
            str := str + ' 男声';

        str := str + ')';

        cobVoice.AddItem(str,nil);
    end;

    with cobCodePage do begin
        AddItem('gb', nil);
        AddItem('big5',nil);
        AddItem('ShiftJIS',nil);
        AddItem('iso8859-1',nil);
        AddItem('Unicode',nil);
        AddItem('Unicode bige',nil);
        AddItem('utf8',nil);
    end;

    jTTS_SetPlay(UINT(-1), Self.Handle, nil, 0);
    Application.OnMessage:= MessageHandler;
    SetLength(gContent, Length(memContent.Text)+1);
    CopyMemory(PChar(gContent), PChar(memContent.Text), Length(memContent.Text));

end;

//播放通知消息处理
procedure TfrmMain.MessageHandler(var Msg: TMsg; var Handled: Boolean);
begin
    if Msg.message = WM_JTTS_NOTIFY then begin
        if Msg.wParam = NOTIFY_END then begin
            ShowMessage('播放完成');
        end;
        Handled:= True;
    end;
end;

procedure TfrmMain.cobVoiceChange(Sender: TObject);
begin
    if cobVoice.ItemIndex>=0 then begin
        lstrcpyA(gjttsConf.szVoiceID, PChar(gVoiceIDs[cobVoice.ItemIndex]));
        jTTS_Set(@gjttsConf);
    end;

end;

procedure TfrmMain.cobCodePageChange(Sender: TObject);
begin
    case cobCodePage.ItemIndex of
    0:  begin
            gjttsConf.nCodePage := CODEPAGE_GB;
            gCharset:= GB2312_CHARSET;
        end;
    1:  begin
            gjttsConf.nCodePage := CODEPAGE_BIG5;
            gCharset:= CHINESEBIG5_CHARSET;
        end;
    2:  begin
            gjttsConf.nCodePage := CODEPAGE_SHIFTJIS;
            gCharset:= SHIFTJIS_CHARSET;
        end;
    3:  begin
            gjttsConf.nCodePage := CODEPAGE_ISO8859_1;
            gCharset:= GB2312_CHARSET;
        end;
    4:  begin
            gjttsConf.nCodePage := CODEPAGE_UNICODE;
            gCharset:= GB2312_CHARSET;
        end;
    5:  begin
            gjttsConf.nCodePage := CODEPAGE_UNICODE_BIGE;
            gCharset:= GB2312_CHARSET;
        end;
    6:  begin
            gjttsConf.nCodePage := CODEPAGE_UTF8;
            gCharset:= DEFAULT_CHARSET;
        end;
    end;
    memContent.Font.Charset:= gCharset;
    if gFileName<>'' then
        memContent.Text:= LoadFileToAnsiString(gFileName, gjttsConf.nCodePage);

    jTTS_Set(@gjttsConf);
end;

procedure TfrmMain.btnOpenClick(Sender: TObject);
begin
    if dlgOpenFile.Execute then begin
        gFileName:= dlgOpenFile.FileName;
        memContent.Text:= LoadFileToAnsiString(gFileName, gjttsConf.nCodePage);
    end;
end;

procedure TfrmMain.btnPauseClick(Sender: TObject);
begin
    jTTS_Pause();
end;

procedure TfrmMain.btnResumeClick(Sender: TObject);
begin
    jTTS_Resume();
end;

procedure TfrmMain.btnStopClick(Sender: TObject);
begin
    jTTS_Stop();
end;

procedure TfrmMain.btnPlayToFileClick(Sender: TObject);
var
    err:ERRCODE;

begin
    err:= jTTS_PlayToFile(PChar(gContent), 'c:\test.wav', FORMAT_WAV,
            @gjttsConf, PLAYTOFILE_ADDHEAD or PLAYMODE_ASYNC, PlayToFileNotify, 0);
    if err<>ERR_NONE then begin
        ShowMessage('jTTS_PlayToFile出错:' + IntToStr(Integer(err)));
    end;
end;

function PlayToFileNotify(wParam:Word; lParam:LongInt; dwUserData:DWORD):BOOL; stdcall;
begin
    if wParam = NOTIFY_END then begin
        MessageBox(0, '合成文件c:\test.wav完成,你可以听听试试.', PChar(frmMain.Caption), MB_OK);
    end;
    PlayToFileNotify:=True;
end;

procedure TfrmMain.btnSessionDataClick(Sender: TObject);
var
    err:ERRCODE;
    pData:PByte;
    dataSize:DWORD;
    pInsert:PINSERTINFO;
    nInsert:Integer;
    BufIndex :Integer;
    BitsPerSample:Integer;
    SamplePerSec:Integer;
    PcmFile:File;
    writeSize:DWORD;
begin
    err:= jTTS_SessionStart(PChar(gContent), @gSessionID, FORMAT_WAV_8K8B,
                    @gjttsConf, 0, @BitsPerSample, @SamplePerSec);

    if err<>ERR_NONE then begin
        ShowMessage('jTTS_SessionStart出错:' + IntToStr(Integer(err)));
        Exit;
    end;

    BufIndex:=0;
    AssignFile(PcmFile, 'c:\test.pcm');
    Rewrite(PcmFile, 1);
    repeat
        BufIndex:= 1 - BufIndex;
        err:= jTTS_SessionGetData(gSessionID, BufIndex, @pData, @dataSize, 0,
                    @pInsert,@nInsert);

        if err= ERR_MORETEXT then begin
            BlockWrite(PcmFile, pData^, dataSize, writeSize);
            if dataSize<>writeSize then begin
                ShowMessage('驱动器c:已满');
                CloseFile(PcmFile);
                jTTS_SessionStop(gSessionID);
                Exit;
            end;
        end;

    until (err<>ERR_MORETEXT);

    CloseFile(PcmFile);
    jTTS_SessionStop(gSessionID);
    ShowMessage('c:\test.pcm合成完毕,是8K,8bit PCM格式的,可以用一些软件听听,如cooledit');

end;


function LoadFileToAnsiString(filename:String; codePage:Integer):String;
var
    textFile:File;
    Buffer: PChar;
    i: Integer;
    iFileLength:Integer;
    iReadBytes:Integer;
    ch:Char;
begin
    if filename='' then begin
        LoadFileToAnsiString:='';
        Exit;
    end;

    iFileLength := 0;
    Buffer := nil;
    try
        AssignFile(textFile, filename);
        Reset(textFile,1);
        iFileLength:= FileSize(textFile);
        Buffer:= PChar(AllocMem(iFileLength + 2));
        Buffer[iFileLength] := Char(0);
        buffer[iFileLength+1] := Char(0);
        BlockRead(textFile, buffer^, iFileLength, iReadBytes);
        SetLength(gContent,iFileLength + 2);
        CopyMemory(PChar(gContent), buffer, iFileLength);
        gContent[iFileLength] := Char(0);
        gContent[iFileLength+1] := Char(0);

        CloseFile(textFile);

    except
        ShowMessage('磁盘I/O错或内存分配错');
        FreeMem(Buffer);
    end;


    if codePage = CODEPAGE_UTF8 then begin
        LoadFileToAnsiString:= UTF8ToAnsi(Buffer);
    end else if codePage = CODEPAGE_UNICODE then begin
        LoadFileToAnsiString:= WideCharToString(PWideChar(buffer));
    end else if codePage = CODEPAGE_UNICODE_BIGE then begin
        for i:=0 to iFileLength+1 do begin
            if i mod 2 <> 0 then begin
                ch:=buffer[i];
                buffer[i]:= buffer[i-1];
                buffer[i-1]:=ch;
            end;
        end;
        LoadFileToAnsiString:= WideCharToString(PWideChar(buffer));
    end else begin
        LoadFileToAnsiString:= buffer;
    end;

    FreeMem(Buffer);

end;



end.
