Add-Type -AssemblyName System.Drawing

$spriteDir = "D:\game drive\super unity in 2d\Assets\Sprites"
if (-not (Test-Path $spriteDir)) {
    New-Item -ItemType Directory -Path $spriteDir -Force | Out-Null
}

function Save-SpriteWithMeta($name, $bmp) {
    $pngPath = Join-Path $spriteDir "$name.png"
    $bmp.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()

    # Create Unity .meta file configured for Sprite2D, Point filter, 16 PPU
    $guid = [System.Guid]::NewGuid().ToString("N")
    $metaContent = @"
fileFormatVersion: 2
guid: $guid
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 1
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMasterTextureLimit: 0
  doesAlphaSplit: 0
  alphaIsTransparency: 1
  spriteMode: 1
  spriteExtrusion: 1
  spriteMeshType: 1
  alignment: 7
  spritePivot: {x: 0.5, y: 0}
  spritePixelsToUnits: 16
  spriteBorder: {x: 0, y: 0, z: 0, w: 0}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  filterMode: 0
  aniso: 1
  mipBias: 0
  wrapU: 1
  wrapV: 1
  wrapW: 1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbook: 0
  platformSettings:
  - serializedVersion: 4
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 0
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
    forceWithoutCompression: 0
"@
    Set-Content -Path "$pngPath.meta" -Value $metaContent -Encoding UTF8
}

function New-BitmapFromLines($w, $h, $pal, $lines) {
    $bmp = New-Object System.Drawing.Bitmap($w, $h, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    for ($y = 0; $y -lt $h; $y++) {
        $line = if ($y -lt $lines.Length) { $lines[$y] } else { "" }
        for ($x = 0; $x -lt $w; $x++) {
            $char = if ($x -lt $line.Length) { [string]$line[$x] } else { "." }
            $color = if ($pal.ContainsKey($char)) { $pal[$char] } else { [System.Drawing.Color]::FromArgb(0,0,0,0) }
            $bmp.SetPixel($x, $y, $color)
        }
    }
    return $bmp
}

# Palettes
$C_TRANS = [System.Drawing.Color]::FromArgb(0,0,0,0)
$C_RED   = [System.Drawing.Color]::FromArgb(255, 230, 37, 34)
$C_BLUE  = [System.Drawing.Color]::FromArgb(255, 0, 102, 230)
$C_SKIN  = [System.Drawing.Color]::FromArgb(255, 252, 188, 120)
$C_BROWN = [System.Drawing.Color]::FromArgb(255, 136, 80, 20)
$C_DARK  = [System.Drawing.Color]::FromArgb(255, 50, 20, 5)
$C_WHITE = [System.Drawing.Color]::FromArgb(255, 255, 255, 255)
$C_YELLOW= [System.Drawing.Color]::FromArgb(255, 252, 216, 0)
$C_GREEN = [System.Drawing.Color]::FromArgb(255, 0, 168, 0)
$C_D_GREEN = [System.Drawing.Color]::FromArgb(255, 0, 100, 0)
$C_BRICK = [System.Drawing.Color]::FromArgb(255, 180, 74, 0)
$C_BRICK_DARK = [System.Drawing.Color]::FromArgb(255, 100, 40, 0)
$C_GOLD  = [System.Drawing.Color]::FromArgb(255, 255, 204, 0)

$marioPal = @{
    "." = $C_TRANS; "R" = $C_RED; "U" = $C_BLUE; "S" = $C_SKIN; "B" = $C_BROWN; "W" = $C_WHITE; "Y" = $C_YELLOW
}
$marioFirePal = @{
    "." = $C_TRANS; "R" = $C_WHITE; "U" = $C_RED; "S" = $C_SKIN; "B" = $C_BROWN; "W" = $C_WHITE; "Y" = $C_YELLOW
}

# --- MARIO SMALL (16x16) ---
$marioIdleLines = @(
    ".....RRRRR......",
    "....RRRRRRRRR...",
    "....BBSSSBSS....",
    "...BSBSSSBSSS...",
    "...BSBSSSBSSS...",
    "...BBSSSSBBBB...",
    ".....SSSSSSS....",
    "....RRURR.......",
    "...RRRURRURRR...",
    "...RRRUUUUURR...",
    "...SSUUUYUURS...",
    "...SSUUUUUUSS...",
    "...SSUUUUUUSS...",
    "....UUU..UUU....",
    "...BBB....BBB...",
    "..BBBB....BBBB.."
)
Save-SpriteWithMeta "mario_small_idle" (New-BitmapFromLines 16 16 $marioPal $marioIdleLines)

$marioRun1Lines = @(
    ".....RRRRR......",
    "....RRRRRRRRR...",
    "....BBSSSBSS....",
    "...BSBSSSBSSS...",
    "...BSBSSSBSSS...",
    "...BBSSSSBBBB...",
    ".....SSSSSSS....",
    "....RRURR.......",
    "...RRRURRURRR...",
    "..SSRRUUUUURS...",
    "..SS.UUUYUURS...",
    ".....UUUUUUSS...",
    "....UUUUUUUU....",
    "...BBB...BBBB...",
    "..BBBB..........",
    "................"
)
Save-SpriteWithMeta "mario_small_run1" (New-BitmapFromLines 16 16 $marioPal $marioRun1Lines)

$marioRun2Lines = @(
    ".....RRRRR......",
    "....RRRRRRRRR...",
    "....BBSSSBSS....",
    "...BSBSSSBSSS...",
    "...BSBSSSBSSS...",
    "...BBSSSSBBBB...",
    ".....SSSSSSS....",
    "....RRURR.......",
    "...RRRURRURRR...",
    "...RRRUUUUURR...",
    "...SSUUUYUURS...",
    "...SSUUUUUUSS...",
    "....UUUUUUUU....",
    "....BBB..BBB....",
    "...BBBB...BBBB..",
    "................"
)
Save-SpriteWithMeta "mario_small_run2" (New-BitmapFromLines 16 16 $marioPal $marioRun2Lines)

$marioRun3Lines = @(
    ".....RRRRR......",
    "....RRRRRRRRR...",
    "....BBSSSBSS....",
    "...BSBSSSBSSS...",
    "...BSBSSSBSSS...",
    "...BBSSSSBBBB...",
    ".....SSSSSSS....",
    "....RRURR.......",
    "...SRRURRURRR...",
    "..SSRRUUUUURR...",
    "..SS.UUUYUUSS...",
    ".....UUUUUU.....",
    "....UUUU..UU....",
    "....BBB....BBB..",
    "............BBBB",
    "................"
)
Save-SpriteWithMeta "mario_small_run3" (New-BitmapFromLines 16 16 $marioPal $marioRun3Lines)

$marioJumpLines = @(
    ".....RRRRR......",
    "....RRRRRRRRR...",
    "....BBSSSBSS....",
    "...BSBSSSBSSS...",
    "...BSBSSSBSSS...",
    "...BBSSSSBBBB...",
    ".....SSSSSSS....",
    "..SS.RRURR......",
    ".SSSRRRURRURR...",
    ".SSSRRRUUUUUR...",
    "..S.SSUUUYUUR...",
    "....SSUUUUUU....",
    "...BBBUUUUUU.BB.",
    "..BBBB.UUUU.BBBB",
    ".......BBB......",
    "................"
)
Save-SpriteWithMeta "mario_small_jump" (New-BitmapFromLines 16 16 $marioPal $marioJumpLines)

$marioSkidLines = @(
    ".....RRRRR......",
    "....RRRRRRRRR...",
    "....BBSSSBSS....",
    "...BSBSSSBSSS...",
    "...BBSSSSBBBB...",
    ".....SSSSSSS....",
    "....RRURR.......",
    "...RRRURRURRR...",
    "...RRRUUUUURR...",
    "...SSUUUYUURS...",
    "...SSUUUUUUSS...",
    "....UUUUUUUU....",
    "...BBBB..BBBB...",
    "..BBBB....BBBB..",
    "................",
    "................"
)
Save-SpriteWithMeta "mario_small_skid" (New-BitmapFromLines 16 16 $marioPal $marioSkidLines)

$marioDieLines = @(
    ".....RRRRR......",
    "....RRRRRRRRR...",
    "....BBSSSBSS....",
    "...BSBSSSBSSS...",
    "...BBSSSSBBBB...",
    ".....SSSSSSS....",
    "...RRRRRRRRRR...",
    "..RRRRRRRRRRRR..",
    ".RRRRRRRRRRRRRR.",
    ".SS.RRRRRRRR.SS.",
    ".SS.RRRRRRRR.SS.",
    "....UUUUUUUU....",
    "...UUUUUUUUUU...",
    "...BBB....BBB...",
    "..BBBB....BBBB..",
    "................"
)
Save-SpriteWithMeta "mario_small_die" (New-BitmapFromLines 16 16 $marioPal $marioDieLines)

# --- SUPER MARIO (16x32) ---
$marioSuperIdleLines = @(
    ".....RRRRR......",
    "....RRRRRRRRR...",
    "....RRRRRRRRR...",
    "....BBSSSBSS....",
    "...BSBSSSBSSS...",
    "...BSBSSSBSSS...",
    "...BBSSSSBBBB...",
    ".....SSSSSSS....",
    ".....SSSSSSS....",
    "....RRURR.......",
    "....RRURR.......",
    "...RRRURRURRR...",
    "...RRRURRURRR...",
    "...RRRUUUUURR...",
    "..SSRRUUUUURRSS.",
    "..SSUUUYUUYUUSS.",
    "..SSUUUUUUUUUSS.",
    "...SUUUUUUUUUS..",
    "....UUUUUUUUU...",
    "....UUUUUUUUU...",
    "....UUU...UUU...",
    "....UUU...UUU...",
    "...UUUU...UUUU..",
    "...UUUU...UUUU..",
    "...BBB.....BBB..",
    "...BBB.....BBB..",
    "..BBBB.....BBBB.",
    "..BBBB.....BBBB.",
    "................",
    "................",
    "................",
    "................"
)
Save-SpriteWithMeta "mario_super_idle" (New-BitmapFromLines 16 32 $marioPal $marioSuperIdleLines)
Save-SpriteWithMeta "mario_fire_idle" (New-BitmapFromLines 16 32 $marioFirePal $marioSuperIdleLines)

Save-SpriteWithMeta "mario_super_run1" (New-BitmapFromLines 16 32 $marioPal $marioSuperIdleLines)
Save-SpriteWithMeta "mario_fire_run1" (New-BitmapFromLines 16 32 $marioFirePal $marioSuperIdleLines)
Save-SpriteWithMeta "mario_super_run2" (New-BitmapFromLines 16 32 $marioPal $marioSuperIdleLines)
Save-SpriteWithMeta "mario_super_jump" (New-BitmapFromLines 16 32 $marioPal $marioSuperIdleLines)
Save-SpriteWithMeta "mario_super_skid" (New-BitmapFromLines 16 32 $marioPal $marioSuperIdleLines)
Save-SpriteWithMeta "mario_super_crouch" (New-BitmapFromLines 16 32 $marioPal $marioSuperIdleLines)

# --- ENEMIES ---
$goombaPal = @{
    "." = $C_TRANS; "B" = $C_BROWN; "D" = $C_DARK; "W" = $C_WHITE; "S" = $C_SKIN
}
$goomba1Lines = @(
    "......BBBB......",
    "....BBBBBBBB....",
    "...BBBBBBBBBB...",
    "..BBBBBBBBBBBB..",
    ".BBBBBBBBBBBBBB.",
    ".BBBBBBBBBBBBBB.",
    "BBBBWWBBBBWWBBBB",
    "BBBWWDWBBWWDWBBB",
    "BBBWWDWBBWWDWBBB",
    "BBBBWWBBBBWWBBBB",
    ".BBBBSSSSSSBBBB.",
    "..BBSSSSSSSSBB..",
    "...SSSSSSSSSS...",
    "...DDDD..DDDD...",
    "..DDDDD..DDDDD..",
    "................"
)
Save-SpriteWithMeta "goomba_walk1" (New-BitmapFromLines 16 16 $goombaPal $goomba1Lines)

$goomba2Lines = @(
    "......BBBB......",
    "....BBBBBBBB....",
    "...BBBBBBBBBB...",
    "..BBBBBBBBBBBB..",
    ".BBBBBBBBBBBBBB.",
    ".BBBBBBBBBBBBBB.",
    "BBBBWWBBBBWWBBBB",
    "BBBWWDWBBWWDWBBB",
    "BBBWWDWBBWWDWBBB",
    "BBBBWWBBBBWWBBBB",
    ".BBBBSSSSSSBBBB.",
    "..BBSSSSSSSSBB..",
    "...SSSSSSSSSS...",
    "....DDDD..DDDD..",
    "...DDDDD..DDDDD.",
    "................"
)
Save-SpriteWithMeta "goomba_walk2" (New-BitmapFromLines 16 16 $goombaPal $goomba2Lines)

$goombaSquished = @(
    "................",
    "................",
    "................",
    "................",
    "................",
    "................",
    "................",
    "......BBBB......",
    "....BBBBBBBB....",
    "...BBWWBBWWBB...",
    "..BBWWDBBWWDBB..",
    ".BBBBWWBBWWBBBB.",
    "BBBBSSSSSSSSBBBB",
    "DDDDDDDDDDDDDDDD",
    "DDDDDDDDDDDDDDDD",
    "................"
)
Save-SpriteWithMeta "goomba_squished" (New-BitmapFromLines 16 16 $goombaPal $goombaSquished)

# Koopa
$koopaPal = @{
    "." = $C_TRANS; "G" = $C_GREEN; "Y" = $C_YELLOW; "W" = $C_WHITE; "D" = $C_DARK; "B" = $C_BROWN
}
$koopaLines = @(
    ".....GGGG.......",
    "....GGGGGG......",
    "...GGGWWDGG.....",
    "..GGGGWWDDGG....",
    "..GGGGGGGGGG....",
    "....YYYYYY......",
    "....YYYYYY......",
    "...GGGGGGGG.....",
    "..GGWWGGWWGG....",
    ".GGWWWWGGWWGG...",
    ".GGWWWWGGWWGG...",
    "..GGWWGGWWGG....",
    "...GGGGGGGG.....",
    "....YYYYYY......",
    "...YYYY.YYYY....",
    "..YYYY...YYYY..."
)
Save-SpriteWithMeta "koopa_walk1" (New-BitmapFromLines 16 16 $koopaPal $koopaLines)
Save-SpriteWithMeta "koopa_walk2" (New-BitmapFromLines 16 16 $koopaPal $koopaLines)

$koopaShellLines = @(
    ".....GGGG.......",
    "...GGGGGGGG.....",
    "..GGGGGGGGGG....",
    ".GGWWGGGGWWGG...",
    ".GGWWWWWWWWGG...",
    "GGWWWWWWWWWWGG..",
    "GGWWGGGGGGWWGG..",
    "GGGGGGGGGGGGGG..",
    "GGGGGGGGGGGGGG..",
    "GGWWGGGGGGWWGG..",
    "GGWWWWWWWWWWGG..",
    ".GGWWWWWWWWGG...",
    ".GGWWGGGGWWGG...",
    "..GGGGGGGGGG....",
    "...GGGGGGGG.....",
    ".....GGGG......."
)
Save-SpriteWithMeta "koopa_shell" (New-BitmapFromLines 16 16 $koopaPal $koopaShellLines)

# Piranha Plant (16x24)
$piranhaPal = @{
    "." = $C_TRANS; "G" = $C_GREEN; "R" = $C_RED; "W" = $C_WHITE; "D" = $C_DARK
}
$piranhaLines = @(
    ".....RRRRRR.....",
    "...RRRRRRRRRR...",
    "..RRRWWRRRWWRR..",
    ".RRRRWWRRRWWRRR.",
    ".RRRRRRRRRRRRRR.",
    "..RRRWWWWWWWRR..",
    "...RRDDDDDDDR...",
    "....RRRRRRRR....",
    ".....GGGGGG.....",
    "....GGGGGGGG....",
    "...GG.GGGG.GG...",
    "..GG..GGGG..GG..",
    "......GGGG......",
    "......GGGG......",
    "......GGGG......",
    "......GGGG......",
    "......GGGG......",
    "......GGGG......",
    "......GGGG......",
    "......GGGG......",
    "......GGGG......",
    "......GGGG......",
    "......GGGG......",
    "......GGGG......"
)
Save-SpriteWithMeta "piranha_plant" (New-BitmapFromLines 16 24 $piranhaPal $piranhaLines)

# --- BLOCKS & ITEMS ---
$qPal = @{
    "." = $C_DARK; "Y" = $C_GOLD; "B" = $C_BROWN; "W" = $C_WHITE; "D" = $C_DARK
}
$q1Lines = @(
    "YYYYYYYYYYYYYYYY",
    "YWWWWWWWWWWWWWWb",
    "YWYYYYYYYYYYYYbb",
    "YWYYYDDDDYYYYYbb",
    "YWYYDDYYDDYYYYbb",
    "YWYYDDYYDDYYYYbb",
    "YWYYYYYDDYYYYYbb",
    "YWYYYYDDYYYYYYbb",
    "YWYYYYDDYYYYYYbb",
    "YWYYYYYYYYYYYYbb",
    "YWYYYYDDYYYYYYbb",
    "YWYYYYDDYYYYYYbb",
    "YWYYYYYYYYYYYYbb",
    "YWYYYYYYYYYYYYbb",
    "Ybbbbbbbbbbbbbbb",
    "bbbbbbbbbbbbbbbb"
).Replace("b", "B")
Save-SpriteWithMeta "block_question_1" (New-BitmapFromLines 16 16 $qPal $q1Lines)
Save-SpriteWithMeta "block_question_2" (New-BitmapFromLines 16 16 $qPal $q1Lines)
Save-SpriteWithMeta "block_question_3" (New-BitmapFromLines 16 16 $qPal $q1Lines)

$emptyBlockLines = @(
    "BBBBBBBBBBBBBBBB",
    "BDDDDDDDDDDDDDDB",
    "BDB..........BDB",
    "BD.B........B.DB",
    "BD..B......B..DB",
    "BD...B....B...DB",
    "BD....B..B....DB",
    "BD.....BB.....DB",
    "BD.....BB.....DB",
    "BD....B..B....DB",
    "BD...B....B...DB",
    "BD..B......B..DB",
    "BD.B........B.DB",
    "BDB..........BDB",
    "BDDDDDDDDDDDDDDB",
    "BBBBBBBBBBBBBBBB"
)
Save-SpriteWithMeta "block_empty" (New-BitmapFromLines 16 16 @{"." = $C_BROWN; "B" = $C_SKIN; "D" = $C_DARK} $emptyBlockLines)

# Brick Block
$brickLines = @(
    "BBBBBBBBDDDDDDDD",
    "BBBBBBBBDDDDDDDD",
    "BBBBBBBBDDDDDDDD",
    "DDDDDDDDDDDDDDDD",
    "DDDDDDDDDDDDDDDD",
    "BBBDDDDDDDDDDBBB",
    "BBBDDDDDDDDDDBBB",
    "DDDDDDDDDDDDDDDD",
    "BBBBBBBBDDDDDDDD",
    "BBBBBBBBDDDDDDDD",
    "BBBBBBBBDDDDDDDD",
    "DDDDDDDDDDDDDDDD",
    "BBBDDDDDDDDDDBBB",
    "BBBDDDDDDDDDDBBB",
    "BBBDDDDDDDDDDBBB",
    "DDDDDDDDDDDDDDDD"
)
Save-SpriteWithMeta "block_brick" (New-BitmapFromLines 16 16 @{"B" = $C_BRICK; "D" = $C_BRICK_DARK} $brickLines)
Save-SpriteWithMeta "brick_debris" (New-BitmapFromLines 8 8 @{"B" = $C_BRICK; "D" = $C_BRICK_DARK} @("BBBBDDDD","BBBBDDDD","DDDDDDDD","DDDDDDDD","BBBBDDDD","BBBBDDDD","DDDDDDDD","DDDDDDDD"))

# Mushroom
$mushPal = @{
    "." = $C_TRANS; "R" = $C_RED; "W" = $C_WHITE; "S" = $C_SKIN; "D" = $C_DARK
}
$mushLines = @(
    ".....RRRRRR.....",
    "...RRRRRRRRRR...",
    "..RRWWWWRRWWWWR.",
    ".RRWWWWWWRWWWWWR",
    ".RRWWWWWWRWWWWWR",
    "RRRWWWWRRRRWWWWR",
    "RRRRRRRRRRRRRRRR",
    "RRRRRRRRRRRRRRRR",
    "..DDSSSSSSSSDD..",
    "...DSSSDDDSSSD..",
    "...DSSSDDDSSSD..",
    "...DSSSSSSSSSD..",
    "...DSSSSSSSSSD..",
    "....DDDDDDDDD...",
    "................",
    "................"
)
Save-SpriteWithMeta "item_mushroom" (New-BitmapFromLines 16 16 $mushPal $mushLines)

# Fire Flower
$flowerPal = @{
    "." = $C_TRANS; "R" = $C_RED; "W" = $C_WHITE; "Y" = $C_GOLD; "G" = $C_GREEN
}
$flowerLines = @(
    ".....YYYYRR.....",
    "....YYYYYYRRR...",
    "...YYWWWWYYRRR..",
    "..YYWWWWWWYYRRR.",
    "..YYWWWWWWYYRRR.",
    "...YYWWWWYYRRR..",
    "....YYYYYYRRR...",
    ".....GGGGGG.....",
    "....GGGGGGGG....",
    "...GG.GGGG.GG...",
    "..GG..GGGG..GG..",
    "......GGGG......",
    "......GGGG......",
    "......GGGG......",
    "................",
    "................"
)
Save-SpriteWithMeta "item_flower" (New-BitmapFromLines 16 16 $flowerPal $flowerLines)

# Fireball
$fbPal = @{
    "." = $C_TRANS; "R" = $C_RED; "Y" = $C_GOLD; "W" = $C_WHITE
}
$fbLines = @(
    "..RRRR..",
    ".RYYYYR.",
    "RYWWWWYR",
    "RYWWWWYR",
    "RYWWWWYR",
    "RYWWWWYR",
    ".RYYYYR.",
    "..RRRR.."
)
Save-SpriteWithMeta "item_fireball" (New-BitmapFromLines 8 8 $fbPal $fbLines)

# Coin
$coinPal = @{
    "." = $C_TRANS; "Y" = $C_GOLD; "W" = $C_WHITE; "D" = $C_DARK
}
$coin1Lines = @(
    ".....YYYYY......",
    "...YYWWWWWYY....",
    "..YYWWYYYYWWY...",
    ".YYWWYYDDYYWWY..",
    ".YYWWYYDDYYWWY..",
    "YYWWYYYDDYYYWWYY",
    "YYWWYYYDDYYYWWYY",
    "YYWWYYYDDYYYWWYY",
    "YYWWYYYDDYYYWWYY",
    "YYWWYYYDDYYYWWYY",
    "YYWWYYYDDYYYWWYY",
    ".YYWWYYDDYYWWY..",
    ".YYWWYYDDYYWWY..",
    "..YYWWYYYYWWY...",
    "...YYWWWWWYY....",
    ".....YYYYY......"
)
Save-SpriteWithMeta "item_coin_1" (New-BitmapFromLines 16 16 $coinPal $coin1Lines)
Save-SpriteWithMeta "item_coin_2" (New-BitmapFromLines 16 16 $coinPal $coin1Lines)
Save-SpriteWithMeta "item_coin_3" (New-BitmapFromLines 16 16 $coinPal $coin1Lines)
Save-SpriteWithMeta "item_coin_4" (New-BitmapFromLines 16 16 $coinPal $coin1Lines)

# Ground Tile (Grass Top)
$groundPal = @{
    "G" = $C_GREEN; "E" = $C_D_GREEN; "B" = $C_BROWN; "D" = $C_DARK
}
$groundLines = @(
    "GGGGGGGGGGGGGGGG",
    "GGGGGGGGGGGGGGGG",
    "GEEEEEEEEEEEEEEG",
    "EEEEEEEEEEEEEEEE",
    "BBBBBBBBBBBBBBBB",
    "BBBBBBDDBBBBBBBB",
    "BBBBBBDDBBBBBBBB",
    "BBBBBBBBBBBBBBBB",
    "BBBBBBBBBBBBDDBB",
    "BBBBBBBBBBBBDDBB",
    "BBBBBBBBBBBBBBBB",
    "BBDDBBBBBBBBBBBB",
    "BBDDBBBBBBBBBBBB",
    "BBBBBBBBBBBBBBBB",
    "BBBBBBBBBBBBBBBB",
    "DDDDDDDDDDDDDDDD"
)
Save-SpriteWithMeta "tile_ground" (New-BitmapFromLines 16 16 $groundPal $groundLines)

# Underground Dirt Tile
$dirtLines = @(
    "BBBBBBBBBBBBBBBB",
    "BBBBBBDDBBBBBBBB",
    "BBBBBBDDBBBBBBBB",
    "BBBBBBBBBBBBBBBB",
    "BBBBBBBBBBBBDDBB",
    "BBBBBBBBBBBBDDBB",
    "BBBBBBBBBBBBBBBB",
    "BBDDBBBBBBBBBBBB",
    "BBDDBBBBBBBBBBBB",
    "BBBBBBBBBBBBBBBB",
    "BBBBBBBBBDDBBBBB",
    "BBBBBBBBBDDBBBBB",
    "BBBBBBBBBBBBBBBB",
    "BBBBDDBBBBBBBBBB",
    "BBBBDDBBBBBBBBBB",
    "DDDDDDDDDDDDDDDD"
)
Save-SpriteWithMeta "tile_dirt" (New-BitmapFromLines 16 16 $groundPal $dirtLines)

# Warp Pipe (32x16 Pipe Top and 32x16 Pipe Body)
$pipePal = @{
    "." = $C_TRANS; "G" = $C_GREEN; "W" = $C_WHITE; "D" = $C_D_GREEN; "B" = $C_DARK
}
$pipeTopLines = @(
    "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB",
    "BWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW ",
    "BWGGGGGGGGGGGGGGGGGGGGGGGGGGGGBB",
    "BWGGGGGGGGGGGGGGGGGGGGGGGGGGGGBB",
    "BWGGWWGGGGGGGGGGGGGGGGGGGGDDDDBB",
    "BWGGWWGGGGGGGGGGGGGGGGGGGGDDDDBB",
    "BWGGWWGGGGGGGGGGGGGGGGGGGGDDDDBB",
    "BWGGWWGGGGGGGGGGGGGGGGGGGGDDDDBB",
    "BWGGWWGGGGGGGGGGGGGGGGGGGGDDDDBB",
    "BWGGWWGGGGGGGGGGGGGGGGGGGGDDDDBB",
    "BWGGWWGGGGGGGGGGGGGGGGGGGGDDDDBB",
    "BWGGWWGGGGGGGGGGGGGGGGGGGGDDDDBB",
    "BWGGWWGGGGGGGGGGGGGGGGGGGGDDDDBB",
    "BWGGWWGGGGGGGGGGGGGGGGGGGGDDDDBB",
    "BWGGGGGGGGGGGGGGGGGGGGGGGGGGGGBB",
    "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB"
)
Save-SpriteWithMeta "pipe_top" (New-BitmapFromLines 32 16 $pipePal $pipeTopLines)

$pipeBodyLines = @(
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB",
    "..BWGGWWGGGGGGGGGGGGGGGGGGDDDDBB"
)
Save-SpriteWithMeta "pipe_body" (New-BitmapFromLines 32 16 $pipePal $pipeBodyLines)

# Flagpole & Flag
$flagPal = @{
    "." = $C_TRANS; "G" = $C_GREEN; "W" = $C_WHITE; "D" = $C_DARK; "Y" = $C_GOLD
}
$flagpoleLines = @(
    ".......YYYY.....",
    "......YYYYYY....",
    "......YYYYYY....",
    ".......YYYY.....",
    "........GG......",
    "........GG......",
    "........GG......",
    "........GG......",
    "........GG......",
    "........GG......",
    "........GG......",
    "........GG......",
    "........GG......",
    "........GG......",
    "........GG......",
    "........GG......"
)
Save-SpriteWithMeta "flagpole" (New-BitmapFromLines 16 16 $flagPal $flagpoleLines)

$flagLines = @(
    "GGGGGGGGGG......",
    "GGGGGGGGGGG.....",
    "GGGGWWGGGGGG....",
    "GGGGWWGGGGGGG...",
    "GGGGWWGGGGGGGG..",
    "GGGGGGGGGGGGGGG.",
    "GGGGGGGGGGGGGGGG",
    "GGGGGGGGGGGGGGG.",
    "GGGGWWGGGGGGGG..",
    "GGGGWWGGGGGGG...",
    "GGGGWWGGGGGG....",
    "GGGGGGGGGGG.....",
    "GGGGGGGGGG......",
    "................",
    "................",
    "................"
)
Save-SpriteWithMeta "flag" (New-BitmapFromLines 16 16 $flagPal $flagLines)

# Castle (80x80)
$castleBmp = New-Object System.Drawing.Bitmap(80, 80, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($castleBmp)
$g.Clear([System.Drawing.Color]::FromArgb(0,0,0,0))
$brickBrush = New-Object System.Drawing.SolidBrush($C_BRICK)
$darkBrush = New-Object System.Drawing.SolidBrush($C_BRICK_DARK)
$doorBrush = New-Object System.Drawing.SolidBrush($C_DARK)

# Base
$g.FillRectangle($brickBrush, 10, 40, 60, 40)
# Battlements
$g.FillRectangle($brickBrush, 10, 32, 12, 8)
$g.FillRectangle($brickBrush, 34, 32, 12, 8)
$g.FillRectangle($brickBrush, 58, 32, 12, 8)
# Tower
$g.FillRectangle($brickBrush, 26, 16, 28, 24)
$g.FillRectangle($brickBrush, 26, 8, 8, 8)
$g.FillRectangle($brickBrush, 46, 8, 8, 8)
# Door
$g.FillRectangle($doorBrush, 30, 56, 20, 24)
$g.FillEllipse($doorBrush, 30, 46, 20, 20)
# Flag on castle
$flagBrush = New-Object System.Drawing.SolidBrush($C_RED)
$g.FillPolygon($flagBrush, @(
    [System.Drawing.Point]::new(40, 0),
    [System.Drawing.Point]::new(28, 6),
    [System.Drawing.Point]::new(40, 12)
))
$g.Dispose()
Save-SpriteWithMeta "castle" $castleBmp

Write-Host "Generated all Mario sprites successfully!"
