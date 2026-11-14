FeatureScript 3112;
import(path : "onshape/std/common.fs", version : "3112.0");

/*
 * KEEEI · Carcasa B «canto rodado» — ESP32 NodeMCU-32S (USB-C) + microswitch KW12-3 con rodillo
 * Mismo mecanismo y electrónica que la carcasa A (cartucho, empujador con leva, protoboard 40x50, USB-C,
 * insertos M3, holguras con los mismos parámetros y valores por defecto). Cambian la forma, la división,
 * el montaje en la pared y los LED.
 * Fuente: modelos-3d/b-organica/carcasa_b.fs (rama carcasa/b-organica). Onshape se actualiza desde git.
 *
 * Ejes:  X = ancho · Y = profundidad (Y=-backT cara de pared, Y=D cara delantera) · Z = altura (Z=0 base)
 * Forma: vista frontal = envolvente de dos círculos (inferior R1, superior R2) unidos por rectas tangentes.
 *        Perfil lateral = chaflán de 45° en cada cara plana + redondeo grande hasta el costado vertical.
 * Piezas (orientación de impresión decidida antes de modelar, todas sin soportes):
 *   "Concha frontal"     cara delantera sobre la cama. Labio de centrado hacia la concha trasera.
 *   "Concha trasera"     cara de pared sobre la cama. Escuadras de la protoboard y bolsillo de la pletina.
 *   "Cartucho mecanismo" tumbado sobre su costado -X (como en la A). Su franja superior es parte de la cúpula.
 *   "Frontal"            placa blanca con el QR, cara trasera sobre la cama, pegada con 2 pasadores de filamento.
 *   "Empujador"          de pie sobre la cara trasera del tope (igual que en la A).
 *   "Pletina de pared"   cara delantera sobre la cama; se atornilla a la pared y la carcasa se cuelga deslizando.
 * Unión de las conchas: 4 tornillos M3 desde la cara de pared a insertos térmicos en la concha frontal.
 * Referencia de impresión: PETG, boquilla 0,4 mm, capa 0,2 mm.
 */

//==CONST_BEGIN
const ZERO         = 0 * millimeter;
const EPS          = 0.2 * millimeter;  // solape para uniones booleanas
const FENCE_T      = 1.2 * millimeter;  // grosor de las escuadras que centran la protoboard
const FENCE_CLR    = 0.3 * millimeter;  // holgura escuadra - pared interior
const FENCE_IN     = 6 * millimeter;    // tamaño de la escuadra/soporte hacia dentro de la placa
const POST_INSET   = 3 * millimeter;    // centro de los postes de apriete desde el canto de la placa
const POST_HALF    = 1.5 * millimeter;  // medio lado de los postes (3 x 3 mm)
const SLEEVE_T     = 1.6 * millimeter;  // pared de la camisa de la ranura
const SHELF_T      = 1.6 * millimeter;  // balda del microswitch
const SLOT_CHAMFER = 0.5 * millimeter;  // chaflán de entrada de la ranura
const M2_CLEAR     = 2.4 * millimeter;  // paso del tornillo M2 en la balda inferior
const M2_PILOT     = 1.7 * millimeter;  // piloto M2 en la balda superior
const RIB_T        = 1.2 * millimeter;  // nervio que arriostra cada poste de apriete
const GUIDE_T      = 1.2 * millimeter;  // pared de la guía del empujador
const COLLAR_X     = 2 * millimeter;    // el tope del empujador sobresale esto a cada lado (X)
const COLLAR_T     = 2 * millimeter;    // grosor del tope (Y)
const PUSH_FLAT    = 0.5 * millimeter;  // parte recta mínima de la punta del empujador
const PUSH_ROUND   = 0.8 * millimeter;  // redondeo donde las rampas nacen del vástago
const PUSH_BED_CH  = 0.3 * millimeter;  // chaflán de cama del empujador
const COUPON_CH    = 0.4 * millimeter;  // chaflán de cama de las piezas de prueba
const PETG_DENSITY = 1.27;              // g/cm³
const MIN_FLOOR    = 0.8 * millimeter;  // suelo/techo mínimo: 4 capas de 0,2 mm
const INLAY_T      = 1.6 * millimeter;  // grosor del frontal pegado (8 capas)
const PIN_D        = 1.9 * millimeter;  // agujero de los pasadores de centrado (filamento de 1,75 mm)
const PIN_IN_BODY  = 1.0 * millimeter;  // profundidad del agujero del pasador en la pared delantera
const PIN_IN_INLAY = 0.8 * millimeter;  // profundidad del agujero del pasador en el frontal
const CAP_GAP      = 0.2 * millimeter;  // juego entre la franja del cartucho y las conchas (cadena no hiperestática)
const SAC_LAYER    = 0.2 * millimeter;  // capa de sacrificio sobre los avellanados (se perfora al montar)
const INLAY_BORDER = 1.6 * millimeter;  // margen del frontal alrededor del QR
const RIB_SUP_T    = 1.6 * millimeter;  // nervio de la pared delantera sobre el que apoya el cartucho
const RIB_SUP_D    = 3 * millimeter;    // cuánto entra ese nervio hacia atrás
const CART_BED_CH  = 0.3 * millimeter;  // chaflán de cama del cartucho y del frontal
const LIP_T        = 1.2 * millimeter;  // grosor del labio de centrado de las conchas (3 líneas)
const LIP_H        = 2.0 * millimeter;  // cuánto entra el labio en la concha trasera
const PAD_T        = 1.2 * millimeter;  // pared que cierra por dentro el bolsillo de la pletina
const PLET_K       = 2.5 * millimeter;  // chaflán 45° del canto inferior delantero de la pletina (y del bolsillo)
const PLET_BED_CH  = 0.4 * millimeter;  // chaflán de cama de la pletina
const TIP_ROOM     = 3.0 * millimeter;  // hueco para la punta del tornillo M3 detrás del inserto
const PLET_TIP     = 0.6 * millimeter;  // la uña del bolsillo no acaba en filo: tramo recto junto a la cara de pared
const DEFAULTS_B = {
        "wall" : 2.0 * millimeter, "wallF" : 2.0 * millimeter,
        "R1" : 32.0 * millimeter, "R2" : 27.0 * millimeter,
        "frontC" : 8.0 * millimeter, "frontR" : 14.0 * millimeter, "backC" : 3.0 * millimeter, "backR" : 6.0 * millimeter,
        "plugClear" : 10.0 * millimeter,
        "pcbW" : 40.0 * millimeter, "pcbH" : 50.0 * millimeter, "pcbT" : 1.6 * millimeter, "pcbY" : 4.0 * millimeter,
        "postGap" : 0.0 * millimeter, "headerH" : 8.5 * millimeter, "espFront" : 4.8 * millimeter, "usbC" : 3.15 * millimeter,
        "espOffX" : 0.0 * millimeter, "usbOW" : 13.5 * millimeter, "usbOT" : 8.0 * millimeter,
        "tagW" : 22.0 * millimeter, "tagT" : 3.0 * millimeter, "slotDepth" : 28.0 * millimeter,
        "fitSlide" : 0.25 * millimeter, "fitFirm" : 0.15 * millimeter,
        "swL" : 20.0 * millimeter, "swT" : 6.0 * millimeter, "swH" : 8.4 * millimeter, "hPressed" : 12.2 * millimeter,
        "hRest" : 20.0 * millimeter, "hOP" : 17.0 * millimeter, "resid" : 0.3 * millimeter, "fitSw" : 0.1 * millimeter,
        "connSpace" : 11.0 * millimeter, "rollerX" : 0.0 * millimeter,
        "swScrews" : true, "holeS" : 9.5 * millimeter, "holeY" : 2.5 * millimeter,
        "pushZ" : 9.0 * millimeter, "restMargin" : 1.5 * millimeter, "overTravel" : 0.8 * millimeter,
        "pushW" : 8.0 * millimeter, "guideL" : 4.0 * millimeter, "fitGuide" : 0.25 * millimeter,
        "rampIn" : 45 * degree, "rampOut" : 52 * degree,
        "backT" : 3.0 * millimeter, "insertD" : 4.0 * millimeter, "insertL" : 6.0 * millimeter, "bossR" : 3.5 * millimeter,
        "screwD" : 3.4 * millimeter, "cbD" : 6.5 * millimeter, "cbDepth" : 1.7 * millimeter,
        "pletT" : 4.0 * millimeter, "pletH" : 9.75 * millimeter, "pletHook" : 2.0 * millimeter,
        "pletScrewD" : 3.8 * millimeter, "pletCsD" : 7.0 * millimeter, "pletScrewS" : 14.0 * millimeter,
        "qrSize" : 30.0 * millimeter, "qrFit" : 0.2 * millimeter, "qrDepth" : 0.4 * millimeter, "qrR" : 2.0 * millimeter,
        "qrZ" : 34.0 * millimeter,
        "leds" : true, "ledD" : 3.1 * millimeter, "ledS" : 16.0 * millimeter,
        "explode" : 0.0 * millimeter, "testMode" : false };
//==CONST_END

// ---------------- Límites de parámetros [mín, defecto, máx] en mm ----------------
const B_WALL      = { (millimeter) : [1.2, 2.0, 5.0] } as LengthBoundSpec;
const B_R1        = { (millimeter) : [20.0, 32.0, 60.0] } as LengthBoundSpec;
const B_R2        = { (millimeter) : [15.0, 27.0, 60.0] } as LengthBoundSpec;
const B_FC        = { (millimeter) : [1.0, 8.0, 15.0] } as LengthBoundSpec;
const B_FR        = { (millimeter) : [0.0, 14.0, 30.0] } as LengthBoundSpec;
const B_BC        = { (millimeter) : [1.0, 3.0, 10.0] } as LengthBoundSpec;
const B_BR        = { (millimeter) : [0.0, 6.0, 20.0] } as LengthBoundSpec;
const B_PLUG      = { (millimeter) : [5.0, 10.0, 40.0] } as LengthBoundSpec;
const B_PCB_W     = { (millimeter) : [26.0, 40.0, 80.0] } as LengthBoundSpec;
const B_PCB_H     = { (millimeter) : [30.0, 50.0, 100.0] } as LengthBoundSpec;
const B_PCB_T     = { (millimeter) : [0.8, 1.6, 2.4] } as LengthBoundSpec;
const B_PCB_Y     = { (millimeter) : [2.0, 4.0, 10.0] } as LengthBoundSpec;
const B_POST_GAP  = { (millimeter) : [-0.3, 0.0, 1.0] } as LengthBoundSpec;
const B_HEADER    = { (millimeter) : [0.0, 8.5, 15.0] } as LengthBoundSpec;
const B_ESP_FRONT = { (millimeter) : [3.0, 4.8, 10.0] } as LengthBoundSpec;
const B_USB_C     = { (millimeter) : [1.0, 3.15, 6.0] } as LengthBoundSpec;
const B_USB_OW    = { (millimeter) : [9.5, 13.5, 20.0] } as LengthBoundSpec;
const B_USB_OT    = { (millimeter) : [4.0, 8.0, 14.0] } as LengthBoundSpec;
const B_ESP_OFF   = { (millimeter) : [-20.0, 0.0, 20.0] } as LengthBoundSpec;
const B_TAG_W     = { (millimeter) : [10.0, 22.0, 40.0] } as LengthBoundSpec;
const B_TAG_T     = { (millimeter) : [1.0, 3.0, 6.0] } as LengthBoundSpec;
const B_SLOT_D    = { (millimeter) : [10.0, 28.0, 45.0] } as LengthBoundSpec;
const B_FIT_SLIDE = { (millimeter) : [0.1, 0.25, 0.6] } as LengthBoundSpec;
const B_FIT_FIRM  = { (millimeter) : [0.0, 0.15, 0.4] } as LengthBoundSpec;
const B_FIT_SW    = { (millimeter) : [0.0, 0.1, 0.4] } as LengthBoundSpec;
const B_SW_L      = { (millimeter) : [10.0, 20.0, 30.0] } as LengthBoundSpec;
const B_SW_T      = { (millimeter) : [3.0, 6.0, 12.0] } as LengthBoundSpec;
const B_SW_H      = { (millimeter) : [4.0, 8.4, 15.0] } as LengthBoundSpec;
const B_SW_PRESS  = { (millimeter) : [5.0, 12.2, 25.0] } as LengthBoundSpec;
const B_SW_REST   = { (millimeter) : [5.0, 20.0, 30.0] } as LengthBoundSpec;
const B_SW_OP     = { (millimeter) : [5.0, 17.0, 30.0] } as LengthBoundSpec;
const B_REST_M    = { (millimeter) : [0.3, 1.5, 4.0] } as LengthBoundSpec;
const B_OVERTRAV  = { (millimeter) : [0.2, 0.8, 4.0] } as LengthBoundSpec;
const B_SW_RESID  = { (millimeter) : [0.0, 0.3, 1.5] } as LengthBoundSpec;
const B_PUSH_Z    = { (millimeter) : [3.0, 9.0, 25.0] } as LengthBoundSpec;
const B_PUSH_W    = { (millimeter) : [3.0, 8.0, 20.0] } as LengthBoundSpec;
const B_FIT_GUIDE = { (millimeter) : [0.1, 0.25, 0.6] } as LengthBoundSpec;
const B_GUIDE_L   = { (millimeter) : [2.0, 4.0, 12.0] } as LengthBoundSpec;
const B_ROLLER_X  = { (millimeter) : [-10.0, 0.0, 10.0] } as LengthBoundSpec;
const B_RAMP_IN   = { (degree) : [20, 45, 55] } as AngleBoundSpec;
const B_RAMP_OUT  = { (degree) : [20, 52, 65] } as AngleBoundSpec;
const B_CONN      = { (millimeter) : [5.0, 11.0, 25.0] } as LengthBoundSpec;
const B_HOLE_S    = { (millimeter) : [3.0, 9.5, 20.0] } as LengthBoundSpec;
const B_HOLE_Y    = { (millimeter) : [1.0, 2.5, 10.0] } as LengthBoundSpec;
const B_BACK_T    = { (millimeter) : [2.0, 3.0, 6.0] } as LengthBoundSpec;
const B_INSERT_D  = { (millimeter) : [2.0, 4.0, 6.0] } as LengthBoundSpec;
const B_INSERT_L  = { (millimeter) : [2.0, 6.0, 12.0] } as LengthBoundSpec;
const B_BOSS_R    = { (millimeter) : [2.0, 3.5, 6.0] } as LengthBoundSpec;
const B_SCREW_D   = { (millimeter) : [2.0, 3.4, 5.0] } as LengthBoundSpec;
const B_CB_D      = { (millimeter) : [4.0, 6.5, 10.0] } as LengthBoundSpec;
const B_CB_DEPTH  = { (millimeter) : [0.0, 1.7, 4.0] } as LengthBoundSpec;
const B_PLET_T    = { (millimeter) : [3.0, 4.0, 8.0] } as LengthBoundSpec;
const B_PLET_H    = { (millimeter) : [6.0, 9.75, 20.0] } as LengthBoundSpec;
const B_PLET_HOOK = { (millimeter) : [1.2, 2.0, 4.0] } as LengthBoundSpec;
const B_PLET_SD   = { (millimeter) : [2.5, 3.8, 5.0] } as LengthBoundSpec;
const B_PLET_CS   = { (millimeter) : [4.0, 7.0, 10.0] } as LengthBoundSpec;
const B_PLET_S    = { (millimeter) : [6.0, 14.0, 40.0] } as LengthBoundSpec;
const B_QR_SIZE   = { (millimeter) : [10.0, 30.0, 60.0] } as LengthBoundSpec;
const B_QR_FIT    = { (millimeter) : [0.0, 0.2, 1.0] } as LengthBoundSpec;
const B_QR_DEPTH  = { (millimeter) : [0.2, 0.4, 1.2] } as LengthBoundSpec;
const B_QR_R      = { (millimeter) : [0.5, 2.0, 6.0] } as LengthBoundSpec;
const B_QR_Z      = { (millimeter) : [10.0, 34.0, 200.0] } as LengthBoundSpec;
const B_LED_D     = { (millimeter) : [1.5, 3.1, 6.0] } as LengthBoundSpec;
const B_LED_S     = { (millimeter) : [4.0, 16.0, 30.0] } as LengthBoundSpec;
const B_EXPLODE   = { (millimeter) : [0.0, 0.0, 100.0] } as LengthBoundSpec;

// Las etiquetas "Name" de los parámetros solo admiten ASCII imprimible: sin tildes, ñ ni °
annotation { "Feature Type Name" : "Carcasa KEEEI B (canto rodado)",
             "Feature Type Description" : "Carcasa organica en 6 piezas (concha frontal, concha trasera, cartucho, frontal QR, empujador y pletina de pared) con el mecanismo de la carcasa A." }
export const carcasaKeeeiB = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Group Name" : "Forma (canto rodado)", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Pared lateral" }
            isLength(definition.wall, B_WALL);
            annotation { "Name" : "Pared delantera" }
            isLength(definition.wallF, B_WALL);
            annotation { "Name" : "Radio del circulo inferior (vista frontal)" }
            isLength(definition.R1, B_R1);
            annotation { "Name" : "Radio del circulo superior (vista frontal)" }
            isLength(definition.R2, B_R2);
            annotation { "Name" : "Cara delantera: chaflan 45 grados" }
            isLength(definition.frontC, B_FC);
            annotation { "Name" : "Cara delantera: radio del redondeo" }
            isLength(definition.frontR, B_FR);
            annotation { "Name" : "Cara de pared: chaflan 45 grados" }
            isLength(definition.backC, B_BC);
            annotation { "Name" : "Cara de pared: radio del redondeo" }
            isLength(definition.backR, B_BR);
            annotation { "Name" : "Hueco minimo para la clavija USB bajo la placa" }
            isLength(definition.plugClear, B_PLUG);
        }
        annotation { "Group Name" : "Protoboard y ESP32", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Protoboard: ancho (X)" }
            isLength(definition.pcbW, B_PCB_W);
            annotation { "Name" : "Protoboard: alto (Z)" }
            isLength(definition.pcbH, B_PCB_H);
            annotation { "Name" : "Protoboard: grosor" }
            isLength(definition.pcbT, B_PCB_T);
            annotation { "Name" : "Altura de los soportes (hueco para soldaduras)" }
            isLength(definition.pcbY, B_PCB_Y);
            annotation { "Name" : "Holgura de los postes de apriete (negativo = presion)" }
            isLength(definition.postGap, B_POST_GAP);
            annotation { "Name" : "Altura de los pines hembra" }
            isLength(definition.headerH, B_HEADER);
            annotation { "Name" : "ESP32: altura del componente mas alto sobre la cara inferior de su PCB" }
            isLength(definition.espFront, B_ESP_FRONT);
            annotation { "Name" : "ESP32: eje del USB sobre la cara inferior de su PCB" }
            isLength(definition.usbC, B_USB_C);
            annotation { "Name" : "ESP32: desplazamiento lateral del USB respecto al centro" }
            isLength(definition.espOffX, B_ESP_OFF);
            annotation { "Name" : "Abertura de la clavija USB: ancho" }
            isLength(definition.usbOW, B_USB_OW);
            annotation { "Name" : "Abertura de la clavija USB: fondo" }
            isLength(definition.usbOT, B_USB_OT);
        }
        annotation { "Group Name" : "Llavero y ranura", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Llavero: ancho" }
            isLength(definition.tagW, B_TAG_W);
            annotation { "Name" : "Llavero: grosor" }
            isLength(definition.tagT, B_TAG_T);
            annotation { "Name" : "Profundidad de la ranura" }
            isLength(definition.slotDepth, B_SLOT_D);
            annotation { "Name" : "Holgura deslizante de la ranura (por lado)" }
            isLength(definition.fitSlide, B_FIT_SLIDE);
            annotation { "Name" : "Holgura de ajuste firme (protoboard, labio; por lado)" }
            isLength(definition.fitFirm, B_FIT_FIRM);
        }
        annotation { "Group Name" : "Microswitch KW12-3", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Cuerpo: largo (X)" }
            isLength(definition.swL, B_SW_L);
            annotation { "Name" : "Rodillo: desplazamiento desde el centro del cuerpo (X)" }
            isLength(definition.rollerX, B_ROLLER_X);
            annotation { "Name" : "Cuerpo: grosor (Z)" }
            isLength(definition.swT, B_SW_T);
            annotation { "Name" : "Cuerpo: alto sin palanca (Y)" }
            isLength(definition.swH, B_SW_H);
            annotation { "Name" : "Alto total con palanca PULSADA" }
            isLength(definition.hPressed, B_SW_PRESS);
            annotation { "Name" : "Alto total con palanca EN REPOSO" }
            isLength(definition.hRest, B_SW_REST);
            annotation { "Name" : "Alto total con la palanca en el punto de clic" }
            isLength(definition.hOP, B_SW_OP);
            annotation { "Name" : "Margen antes del tope (sin aplastar la palanca)" }
            isLength(definition.resid, B_SW_RESID);
            annotation { "Name" : "Holgura del cuerpo entre balda y techo (por lado)" }
            isLength(definition.fitSw, B_FIT_SW);
            annotation { "Name" : "Espacio detras para conector 2x1 y cables" }
            isLength(definition.connSpace, B_CONN);
            annotation { "Name" : "Fijar con 2 tornillos M2 por sus agujeros de montaje", "Default" : true }
            definition.swScrews is boolean;
            if (definition.swScrews)
            {
                annotation { "Name" : "Agujeros de montaje: distancia entre centros" }
                isLength(definition.holeS, B_HOLE_S);
                annotation { "Name" : "Agujeros de montaje: distancia desde la cara de las patillas" }
                isLength(definition.holeY, B_HOLE_Y);
            }
        }
        annotation { "Group Name" : "Empujador", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Altura del empujador sobre el fondo de la ranura" }
            isLength(definition.pushZ, B_PUSH_Z);
            annotation { "Name" : "Margen en reposo por encima del clic" }
            isLength(definition.restMargin, B_REST_M);
            annotation { "Name" : "Recorrido tras el clic con el llavero dentro" }
            isLength(definition.overTravel, B_OVERTRAV);
            annotation { "Name" : "Empujador: ancho (X)" }
            isLength(definition.pushW, B_PUSH_W);
            annotation { "Name" : "Empujador: largo de la guia" }
            isLength(definition.guideL, B_GUIDE_L);
            annotation { "Name" : "Holgura del empujador en su guia (por lado)" }
            isLength(definition.fitGuide, B_FIT_GUIDE);
            annotation { "Name" : "Rampa de entrada (arriba): angulo desde la vertical" }
            isAngle(definition.rampIn, B_RAMP_IN);
            annotation { "Name" : "Rampa de salida (abajo): angulo desde la vertical" }
            isAngle(definition.rampOut, B_RAMP_OUT);
        }
        annotation { "Group Name" : "Union de las conchas", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Grosor de la pared trasera" }
            isLength(definition.backT, B_BACK_T);
            annotation { "Name" : "Inserto M3: diametro del agujero" }
            isLength(definition.insertD, B_INSERT_D);
            annotation { "Name" : "Inserto M3: profundidad del agujero" }
            isLength(definition.insertL, B_INSERT_L);
            annotation { "Name" : "Radio de los tetones de los insertos" }
            isLength(definition.bossR, B_BOSS_R);
            annotation { "Name" : "Tornillo: diametro de paso" }
            isLength(definition.screwD, B_SCREW_D);
            annotation { "Name" : "Avellanado plano: diametro" }
            isLength(definition.cbD, B_CB_D);
            annotation { "Name" : "Avellanado plano: profundidad" }
            isLength(definition.cbDepth, B_CB_DEPTH);
        }
        annotation { "Group Name" : "Pletina de pared", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Pletina: grosor" }
            isLength(definition.pletT, B_PLET_T);
            annotation { "Name" : "Pletina: alto" }
            isLength(definition.pletH, B_PLET_H);
            annotation { "Name" : "Pletina: bisel de enganche (45 grados)" }
            isLength(definition.pletHook, B_PLET_HOOK);
            annotation { "Name" : "Tornillo de pared: diametro de paso" }
            isLength(definition.pletScrewD, B_PLET_SD);
            annotation { "Name" : "Tornillo de pared: diametro de la cabeza avellanada" }
            isLength(definition.pletCsD, B_PLET_CS);
            annotation { "Name" : "Tornillos de pared: distancia entre centros" }
            isLength(definition.pletScrewS, B_PLET_S);
        }
        annotation { "Group Name" : "Frontal QR y LED", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Etiqueta QR: lado" }
            isLength(definition.qrSize, B_QR_SIZE);
            annotation { "Name" : "Etiqueta QR: holgura" }
            isLength(definition.qrFit, B_QR_FIT);
            annotation { "Name" : "Etiqueta QR: profundidad del rebaje" }
            isLength(definition.qrDepth, B_QR_DEPTH);
            annotation { "Name" : "Etiqueta QR: radio de esquinas" }
            isLength(definition.qrR, B_QR_R);
            annotation { "Name" : "Etiqueta QR: altura del centro" }
            isLength(definition.qrZ, B_QR_Z);
            annotation { "Name" : "Agujeros para LED en la cupula, junto a la ranura", "Default" : true }
            definition.leds is boolean;
            if (definition.leds)
            {
                annotation { "Name" : "LED: diametro del agujero" }
                isLength(definition.ledD, B_LED_D);
                annotation { "Name" : "LED: separacion entre centros" }
                isLength(definition.ledS, B_LED_S);
            }
        }
        annotation { "Name" : "Separar piezas (vista explosionada)" }
        isLength(definition.explode, B_EXPLODE);
        annotation { "Name" : "Piezas de prueba (recortes ligeros para ajustar holguras)", "Default" : false }
        definition.testMode is boolean;
    }
    {
        construirCarcasaB(context, id, definition);
    }, DEFAULTS_B);

//==FUNCS_BEGIN
// Prisma recto entre dos esquinas opuestas ("box" es palabra reservada en FeatureScript)
function caja(context is Context, id is Id, p0 is Vector, p1 is Vector)
{
    fCuboid(context, id, {
                "corner1" : vector(min(p0[0], p1[0]), min(p0[1], p1[1]), min(p0[2], p1[2])),
                "corner2" : vector(max(p0[0], p1[0]), max(p0[1], p1[1]), max(p0[2], p1[2])) });
}

// Cilindro con eje paralelo a Y (centro en x, z)
function cylY(context is Context, id is Id, x, z, y0, y1, r)
{
    fCylinder(context, id, { "bottomCenter" : vector(x, y0, z), "topCenter" : vector(x, y1, z), "radius" : r });
}

// Cilindro con eje paralelo a Z (centro en x, y)
function cylZ(context is Context, id is Id, x, y, z0, z1, r)
{
    fCylinder(context, id, { "bottomCenter" : vector(x, y, z0), "topCenter" : vector(x, y, z1), "radius" : r });
}

// Sólido que contiene un punto, solo entre los sólidos que crea esta feature
function bodyAt(id is Id, p is Vector) returns Query
{
    return qContainsPoint(qBodyType(qCreatedBy(id, EntityType.BODY), BodyType.SOLID), p);
}

function subtract(context is Context, id is Id, target is Query, tools is Query)
{
    opBoolean(context, id, { "tools" : tools, "targets" : target, "operationType" : BooleanOperationType.SUBTRACTION });
}

function unite(context is Context, id is Id, bodies is Query)
{
    opBoolean(context, id, { "tools" : bodies, "operationType" : BooleanOperationType.UNION });
}

// ¿Está el punto p = (x relativo al eje, z) dentro del contorno frontal desplazado m hacia dentro?
// El contorno es la envolvente convexa de los círculos (0, z1, R1) y (0, z2, R2).
function dentroGota(p is Vector, g is map, m) returns boolean
{
    const x = abs(p[0]);
    const z = p[1];
    const r1 = g.R1 - m;
    const r2 = g.R2 - m;
    var dentro = false;
    if (r1 > ZERO && r2 > ZERO)
    {
        if (sqrt(x * x + (z - g.z1) * (z - g.z1)) <= r1 || sqrt(x * x + (z - g.z2) * (z - g.z2)) <= r2)
        {
            dentro = true;
        }
        else
        {
            const s = (g.R1 - g.R2) / (g.z2 - g.z1);
            const c = sqrt(1 - s * s);
            if (-s * x + c * (z - g.z1) >= ZERO && -s * x + c * (z - g.z2) <= ZERO)
                dentro = (c * x + s * (z - g.z1) <= r1);
        }
    }
    return dentro;
}

// Chaflán de 45° de ancho c en las aristas de la cara plana y = yFace y redondeo r entre ese chaflán y
// el costado (sgn = +1 si el sólido crece hacia +Y desde esa cara). Con c >= 0,3·r el redondeo cabe en el chaflán.
function redondeo(context is Context, id is Id, body is Query, yFace, sgn is number, c, r)
{
    if (c > 0.01 * millimeter)
    {
        opChamfer(context, id + "ch", {
                    "entities" : qCoincidesWithPlane(qOwnedByBody(body, EntityType.EDGE), plane(vector(ZERO, yFace, ZERO), Y_DIRECTION)),
                    "chamferType" : ChamferType.EQUAL_OFFSETS,
                    "width" : c });
        if (r > 0.01 * millimeter)
        {
            opFillet(context, id + "fi", {
                        "entities" : qCoincidesWithPlane(qOwnedByBody(body, EntityType.EDGE), plane(vector(ZERO, yFace + sgn * c, ZERO), Y_DIRECTION)),
                        "radius" : r });
        }
    }
}

// Sólido "canto rodado": contorno frontal (envolvente de los dos círculos) desplazado m hacia dentro,
// extruido en Y de y0 (cara de pared) a y1 (cara delantera), con chaflán + redondeo en cada cara.
// El perfil lateral pasa de 45° en la cara a vertical en el costado: imprimiendo sobre una cara plana
// ningún punto de la superficie supera los 45° de voladizo.
function gota(context is Context, id is Id, g is map, cx, m, y0, y1, cB, rB, cF, rF) returns Query
{
    const r1 = g.R1 - m;
    const r2 = g.R2 - m;
    const s = (g.R1 - g.R2) / (g.z2 - g.z1);
    const c = sqrt(1 - s * s);
    // Plano de croquis con normal -Y y eje u = X: el eje v queda en +Z
    const sk = newSketchOnPlane(context, id + "sk", { "sketchPlane" : plane(vector(ZERO, y0, ZERO), -Y_DIRECTION, X_DIRECTION) });
    skArc(sk, "inf", { "start" : vector(cx - r1 * c, g.z1 + r1 * s), "mid" : vector(cx, g.z1 - r1), "end" : vector(cx + r1 * c, g.z1 + r1 * s) });
    skLineSegment(sk, "der", { "start" : vector(cx + r1 * c, g.z1 + r1 * s), "end" : vector(cx + r2 * c, g.z2 + r2 * s) });
    skArc(sk, "sup", { "start" : vector(cx + r2 * c, g.z2 + r2 * s), "mid" : vector(cx, g.z2 + r2), "end" : vector(cx - r2 * c, g.z2 + r2 * s) });
    skLineSegment(sk, "izq", { "start" : vector(cx - r2 * c, g.z2 + r2 * s), "end" : vector(cx - r1 * c, g.z1 + r1 * s) });
    skSolve(sk);
    opExtrude(context, id + "ext", {
                "entities" : qSketchRegion(id + "sk"),
                "direction" : Y_DIRECTION,
                "endBound" : BoundingType.BLIND,
                "endDepth" : y1 - y0 });
    opDeleteBodies(context, id + "skDel", { "entities" : qCreatedBy(id + "sk", EntityType.BODY) });
    const body = qCreatedBy(id + "ext", EntityType.BODY);
    redondeo(context, id + "rB", body, y0, 1, cB, rB);
    redondeo(context, id + "rF", body, y1, -1, cF, rF);
    return body;
}

// Recorta el sólido target con la envolvente exterior (nada asoma fuera del canto rodado)
function recortarExterior(context is Context, id is Id, target is Query, g is map, cx, yw, D, definition is map)
{
    const o = gota(context, id + "o", g, cx, ZERO, yw, D, definition.backC, definition.backR, definition.frontC, definition.frontR);
    opBoolean(context, id + "int", { "tools" : qUnion([target, o]), "operationType" : BooleanOperationType.INTERSECTION });
}

// Chaflán de cama arista a arista (si una no admite chaflán, se salta y sigue con las demás).
// Solo en aristas entre caras planas a 90° de la cama: en una arista inclinada o curva el chaflán
// dejaría una carita en voladizo. Devuelve cuántas se han achaflanado.
function chaflanCama(context is Context, id is Id, body is Query, edges is Query, bedNormal is Vector, w) returns number
{
    var pts = [];
    for (var e in evaluateQuery(context, edges))
    {
        var recta = true;
        for (var f in evaluateQuery(context, qAdjacent(e, AdjacencyType.EDGE, EntityType.FACE)))
        {
            var pn = undefined;
            try silent
            {
                pn = evPlane(context, { "face" : f }).normal;
            }
            if (pn == undefined || (abs(dot(pn, bedNormal)) > 0.05 && abs(dot(pn, bedNormal)) < 0.99))
                recta = false;
        }
        if (recta)
            pts = append(pts, evEdgeTangentLine(context, { "edge" : e, "parameter" : 0.5 }).origin);
    }
    var ok = 0;
    for (var k = 0; k < size(pts); k += 1)
    {
        try silent
        {
            opChamfer(context, id + ("e" ~ toString(k)), {
                        "entities" : qContainsPoint(qOwnedByBody(body, EntityType.EDGE), pts[k]),
                        "chamferType" : ChamferType.EQUAL_OFFSETS,
                        "width" : w });
            ok += 1;
        }
    }
    return ok;
}

// Edges de un sólido sobre un plano de cama que son rectas paralelas a X o Z (cortes de caja)
function aristasCorte(body is Query, pl is Plane) returns Query
{
    const e = qCoincidesWithPlane(qOwnedByBody(body, EntityType.EDGE), pl);
    return qUnion([qParallelEdges(e, X_DIRECTION), qParallelEdges(e, Z_DIRECTION)]);
}

function construirCarcasaB(context is Context, id is Id, definition is map)
{
    // ====================== 1. COTAS DERIVADAS ======================
    // --- Mecanismo (igual que la A) ---
    const wall = definition.wall;
    const wallF = definition.wallF;
    const slotHalf = definition.tagW / 2 + definition.fitSlide;
    const slotW = definition.tagT + 2 * definition.fitSlide;
    const sleeveHalf = slotHalf + SLEEVE_T;
    const leverRest = definition.hOP + definition.restMargin;
    const leverIn = definition.hOP - definition.overTravel;
    const pushTravel = leverRest - leverIn;
    const pushOut = pushTravel + (slotW - definition.tagT);
    const hP = definition.swT + 2 * definition.fitSw - 2 * definition.fitGuide;
    const rampZin = pushOut / tan(definition.rampIn);
    const rampZout = pushOut / tan(definition.rampOut);
    const tipFlat = hP - rampZin - rampZout;
    const D1 = definition.connSpace + leverRest + COLLAR_T + definition.guideL + SLEEVE_T + slotW + SLEEVE_T + wallF;
    const espFrontY = definition.pcbY + definition.pcbT + definition.headerH + definition.espFront;
    const D2 = espFrontY + 1 * millimeter + SLEEVE_T + slotW + SLEEVE_T + wallF;
    const D = max(D1, D2);
    const ysF = D - wallF;
    const ysS = ysF - SLEEVE_T;
    const ysB = ysS - slotW;
    const yGuideB = ysB - SLEEVE_T - definition.guideL;
    const yCollarB = yGuideB - COLLAR_T;
    const yTip = ysB + pushOut;
    const yb = yCollarB - leverRest;
    const yw = -definition.backT;                                        // cara de pared (cama de la concha trasera)

    // --- Forma ---
    const R1 = definition.R1;
    const R2 = definition.R2;
    const cx = max(R1, R2);
    const S2 = sqrt(2) - 1;
    // Redondeos interiores: el exacto a pared constante, o algo más metido si el chaflán no deja sitio al redondeo
    const riF = (definition.frontR - wall > 0.5 * millimeter) ? definition.frontR - wall : ZERO;
    const ciF = max(definition.frontC + S2 * wall - wallF, 0.3 * riF + 0.1 * millimeter);
    const riB = (definition.backR - wall > 0.5 * millimeter) ? definition.backR - wall : ZERO;
    const ciB = max(definition.backC + S2 * wall - definition.backT, 0.3 * riB + 0.1 * millimeter);
    // La protoboard baja todo lo que permite el círculo inferior (sus escuadras a Mf del contorno)
    const z1 = R1;
    const xf = definition.pcbW / 2 + definition.fitFirm + FENCE_T;
    const Mf = wall + ciB + FENCE_CLR + 0.2 * millimeter;
    if (R1 - Mf < xf + 0.5 * millimeter)
        throw regenError("Radio inferior demasiado pequeño: la protoboard no cabe a lo ancho.");
    const zB0 = max(wall + definition.plugClear, z1 - sqrt((R1 - Mf) ^ 2 - xf ^ 2) + definition.fitFirm + FENCE_T);
    const zB1 = zB0 + definition.pcbH;
    const fenceTop = zB1 + definition.fitFirm + FENCE_T;
    const shelfTop = fenceTop + 1 * millimeter + SHELF_T;
    const shelf2Bot = shelfTop + definition.swT + 2 * definition.fitSw;
    const zc = shelfTop + definition.fitSw + definition.swT / 2;
    const zSlotBottom = zc - definition.pushZ;
    const zTop = zSlotBottom + definition.slotDepth + wall;              // cima de la cúpula (eje)
    const z2 = zTop - R2;
    if (z2 - z1 < abs(R1 - R2) + 2 * millimeter)
        throw regenError("Forma imposible: los dos círculos del contorno quedan demasiado juntos. Reduce la diferencia entre los radios.");
    const g = { "z1" : z1, "R1" : R1, "z2" : z2, "R2" : R2 };
    const swX = cx - definition.rollerX;
    const usbX = cx + definition.espOffX;
    const usbY = definition.pcbY + definition.pcbT + definition.headerH + definition.usbC;
    const ySplit = usbY;                                                 // plano de unión: por el centro del paso USB
    const zCart0 = zSlotBottom - SLEEVE_T;
    // Muesca de la cúpula: por debajo, el cartucho apoya en la parte plana de la pared delantera interior
    const needN = sleeveHalf + definition.fitSlide + 0.3 * millimeter;
    const rrN = R2 - wall - ciF;
    if (rrN < needN + 0.5 * millimeter)
        throw regenError("Radio superior demasiado pequeño para el cartucho del mecanismo.");
    const zNotch = z2 + sqrt(rrN ^ 2 - needN ^ 2);
    // Tetones: 2 a los lados de la protoboard y 2 junto al cartucho, por encima de la placa
    const sbX = definition.pcbW / 2 + definition.fitFirm + 0.5 * millimeter + definition.bossR;
    const sbZ = z1;
    const tbX = sleeveHalf + definition.fitSlide + 0.5 * millimeter + definition.bossR;
    const tbZ = fenceTop + 0.5 * millimeter + definition.bossR;
    const bossPts = [[cx - sbX, sbZ], [cx + sbX, sbZ], [cx - tbX, tbZ], [cx + tbX, tbZ]];
    // Frontal (QR) pegado sobre la cara delantera
    const qrHalf = definition.qrSize / 2 + definition.qrFit;
    const inlayHalf = qrHalf + INLAY_BORDER;
    const inlayZ0 = definition.qrZ - qrHalf - INLAY_BORDER;
    const inlayZ1 = definition.qrZ + qrHalf + PIN_D + 2 * MIN_FLOOR;
    const pinX = qrHalf / 2;
    const pinZ = definition.qrZ + qrHalf + MIN_FLOOR + PIN_D / 2;
    const inlayR = min(definition.qrR + INLAY_BORDER, inlayHalf - 0.5 * millimeter);
    // LED: en la cúpula, detrás de la boca de la ranura
    const ledY = ysB - SLEEVE_T - definition.ledD / 2 - 1 * millimeter;
    // Pletina y su bolsillo en la cara de pared
    const ph = tbX - definition.cbD / 2 - PAD_T;                         // medio ancho del bolsillo
    const pw = ph - definition.fitSlide;                                 // medio ancho de la pletina
    const travel = definition.pletHook + definition.fitSlide;            // cuánto se levanta la carcasa para colgarla
    const zbP = zB1 + definition.fitFirm + 0.3 * millimeter + PAD_T;     // fondo del bolsillo (sobre la protoboard)
    const zp0 = zbP + travel + definition.fitSlide;                      // base de la pletina colgada
    const ztP = zp0 + definition.pletH + definition.fitSlide;            // techo del bolsillo (lado profundo)
    const dP = definition.pletT + definition.fitSlide;                   // profundidad del bolsillo
    const padTop = yw + dP + PAD_T;
    const zsP = definition.pletH - definition.pletHook - definition.pletScrewD / 2 - MIN_FLOOR;  // eje de los tornillos de pared

    // ====================== 2. COMPROBACIONES ======================
    if (pushOut > slotW - 0.4 * millimeter)
        throw regenError("El empujador ocuparía toda la ranura: reduce el margen en reposo o el recorrido tras el clic.");
    if (definition.hRest < leverRest + 0.2 * millimeter)
        throw regenError("En reposo la palanca no llega al empujador: el margen sobre el clic es mayor que el recorrido libre.");
    if (leverIn < definition.hPressed + definition.resid)
        throw regenError("Con el llavero dentro la palanca llegaría al final de carrera: reduce el recorrido tras el clic.");
    if (leverIn < definition.swH + 1 * millimeter)
        throw regenError("El tope del empujador choca con el cuerpo del microswitch.");
    if (tipFlat < PUSH_FLAT)
        throw regenError("Las dos rampas del empujador no caben en su alto: aumenta sus ángulos o reduce el margen o el recorrido.");
    if (definition.pushW > 2 * slotHalf - 1 * millimeter)
        throw regenError("El empujador es más ancho que la ranura.");
    if (yb < definition.connSpace - 0.01 * millimeter)
        throw regenError("No queda espacio detrás del microswitch para el conector.");
    if (definition.pcbW / 2 - POST_INSET - POST_HALF < sleeveHalf + 0.5 * millimeter)
        throw regenError("La protoboard es demasiado estrecha: los postes de apriete chocan con el cartucho.");
    if (abs(swX - cx) + definition.swL / 2 + 0.5 * millimeter > sleeveHalf)
        throw regenError("El microswitch no cabe a lo ancho del cartucho.");
    if (definition.swScrews && definition.holeS / 2 + M2_CLEAR / 2 > definition.swL / 2)
        throw regenError("Los agujeros de montaje del microswitch quedan fuera de su cuerpo.");
    if (usbY - definition.usbOT / 2 < 0.5 * millimeter || usbY + definition.usbOT / 2 > D - wallF - 0.5 * millimeter)
        throw regenError("La abertura del USB no cabe en el fondo de la carcasa.");
    if (zSlotBottom - SLEEVE_T < zB0)
        throw regenError("La ranura es demasiado profunda para la altura de la carcasa.");
    if (definition.frontR > 0.01 * millimeter && definition.frontC < 0.3 * definition.frontR)
        throw regenError("Cara delantera: el chaflán debe medir al menos 0,3 veces el radio del redondeo.");
    if (definition.backR > 0.01 * millimeter && definition.backC < 0.3 * definition.backR)
        throw regenError("Cara de pared: el chaflán debe medir al menos 0,3 veces el radio del redondeo.");
    if (yw + definition.backC + 0.42 * definition.backR > ySplit - LIP_H - 1 * millimeter ||
        D - definition.frontC - 0.42 * definition.frontR < ySplit + 1 * millimeter)
        throw regenError("Los redondeos llegan al plano de unión: redúcelos.");
    if (!dentroGota(vector(xf, fenceTop), g, Mf))
        throw regenError("La protoboard no cabe por arriba: aumenta el radio superior.");
    if (!dentroGota(vector(needN, min(zCart0, shelfTop - SHELF_T) - RIB_SUP_T), g, wall + ciF))
        throw regenError("El cartucho no cabe por abajo dentro de la pared delantera: aumenta los radios.");
    if (shelf2Bot + SHELF_T > zNotch)
        throw regenError("Las baldas del microswitch no caben bajo la cúpula: aumenta el radio superior.");
    if (tbZ + definition.bossR + 0.7 * millimeter > zNotch - CAP_GAP)
        throw regenError("Los tetones superiores quedan dentro de la muesca del cartucho.");
    for (var i = 0; i < 4; i += 1)
    {
        const p = vector(bossPts[i][0] - cx, bossPts[i][1]);
        if (!dentroGota(p, g, definition.backC + definition.cbD / 2 + MIN_FLOOR))
            throw regenError("Un avellanado de la cara de pared queda demasiado cerca del borde: aumenta los radios.");
        if (!dentroGota(p, g, wall + definition.fitFirm + LIP_T + definition.bossR + 0.5 * millimeter))
            throw regenError("Un tetón choca con el labio de centrado: aumenta los radios.");
    }
    if (!dentroGota(vector(inlayHalf, inlayZ0), g, definition.frontC + 0.5 * millimeter) ||
        !dentroGota(vector(inlayHalf, inlayZ1), g, definition.frontC + 0.5 * millimeter))
        throw regenError("El frontal con el QR no cabe en la cara delantera plana: muévelo o reduce el chaflán delantero.");
    if (wallF - PIN_IN_BODY < MIN_FLOOR - 0.01 * millimeter)
        throw regenError("Pared delantera demasiado fina para los pasadores del frontal (mínimo 1,8 mm).");
    if (definition.qrDepth > INLAY_T - MIN_FLOOR + 0.01 * millimeter)
        throw regenError("El rebaje de la etiqueta QR es demasiado profundo para el frontal (máximo 0,8 mm).");
    if (pinZ + PIN_D / 2 > zCart0 - RIB_SUP_T)
        throw regenError("La etiqueta QR queda demasiado alta: sus pasadores llegan al nervio del cartucho.");
    if (definition.cbDepth > definition.backT - 0.8 * millimeter)
        throw regenError("El avellanado es demasiado profundo para el grosor de la pared trasera.");
    if (definition.leds)
    {
        if (definition.ledS / 2 + definition.ledD / 2 + MIN_FLOOR > sleeveHalf)
            throw regenError("Los LED quedan fuera de la franja del cartucho: júntalos.");
        if (definition.ledS / 2 - definition.ledD / 2 < definition.pushW / 2 + definition.fitGuide + GUIDE_T + 0.3 * millimeter)
            throw regenError("Los LED quedan encima de la guía del empujador: sepáralos.");
        if (ledY - definition.ledD / 2 < ySplit + CAP_GAP + MIN_FLOOR)
            throw regenError("Los LED no caben entre la ranura y la concha trasera.");
    }
    if (pw < definition.pletScrewS / 2 + definition.pletCsD / 2 + MIN_FLOOR)
        throw regenError("Los tornillos de pared no caben en la pletina: júntalos.");
    if (zsP - definition.pletCsD / 2 < ZERO)
        throw regenError("La pletina es demasiado baja para la cabeza de sus tornillos.");
    if (!dentroGota(vector(ph + PAD_T, ztP + PAD_T), g, wall + ciB + 0.3 * millimeter) ||
        !dentroGota(vector(ph, ztP), g, definition.backC + MIN_FLOOR))
        throw regenError("El bolsillo de la pletina no cabe en la cara de pared.");
    if (ztP - zbP - PLET_K > 10.01 * millimeter)
        reportFeatureWarning(context, id, "El techo del bolsillo de la pletina es un puente de más de 10 mm: reduce su alto o el bisel.");
    if (yb - padTop < 6 * millimeter)
        reportFeatureWarning(context, id, "Queda poco sitio entre el bolsillo de la pletina y el microswitch para el conector.");
    if (!definition.swScrews)
        reportFeatureWarning(context, id, "Sin los tornillos M2 nada impide que la palanca empuje el microswitch hacia atrás: hay que pegarlo a las baldas.");

    // Puntos interiores que identifican cada pieza
    const bodyPoint = vector(cx, D - wallF / 2, z1);                     // pared delantera
    const backPoint = vector(cx, yw / 2, z1);                            // pared trasera
    const cartPoint = vector(cx, ySplit + CAP_GAP + 1 * millimeter, zTop - wall / 2);   // franja superior del cartucho
    const inlayPoint = vector(cx, D + 0.3 * millimeter, definition.qrZ);
    const pushPoint = vector(cx, ysB - SLEEVE_T / 2, zc);
    const pletPoint = vector(cx, yw + 1 * millimeter, zp0 + 1 * millimeter);

    // ====================== 3. CONCHA FRONTAL ======================
    // Se imprime con la cara delantera sobre la cama: todo crece hacia -Y
    const Of = gota(context, id + "of", g, cx, ZERO, yw, D, definition.backC, definition.backR, definition.frontC, definition.frontR);
    const If = gota(context, id + "if", g, cx, wall, ZERO, ysF, ciB, riB, ciF, riF);
    subtract(context, id + "fShell", Of, If);
    caja(context, id + "fBackBox", vector(-1 * millimeter, yw - 1 * millimeter, -1 * millimeter), vector(2 * cx + 1 * millimeter, ySplit, zTop + 10 * millimeter));
    subtract(context, id + "fSplit", Of, qCreatedBy(id + "fBackBox", EntityType.BODY));
    // Tetones de los insertos (desde la pared delantera hasta el plano de unión)
    for (var i = 0; i < 4; i += 1)
    {
        cylY(context, id + "fin" + ("boss" ~ toString(i)), bossPts[i][0], bossPts[i][1], ySplit, ysF + EPS, definition.bossR);
    }
    // Nervio de la pared delantera sobre el que apoya el cartucho
    caja(context, id + "fin" + "cartRib",
        vector(cx - sleeveHalf, ysF - RIB_SUP_D, zCart0 - RIB_SUP_T),
        vector(cx + sleeveHalf, ysF + EPS, zCart0));
    // Postes que aprietan la protoboard (cruzan el plano de unión) y nervios hasta la pared
    const postX = [cx - (definition.pcbW / 2 - POST_INSET), cx + (definition.pcbW / 2 - POST_INSET)];
    const postZ = [zB0 + POST_INSET, zB1 - POST_INSET];
    var n = 0;
    for (var px in postX)
    {
        for (var pz in postZ)
        {
            caja(context, id + "fin" + ("post" ~ toString(n)),
                vector(px - POST_HALF, definition.pcbY + definition.pcbT + definition.postGap, pz - POST_HALF),
                vector(px + POST_HALF, ysF + EPS, pz + POST_HALF));
            const wallX = (px < cx) ? -1 * millimeter : 2 * cx + 1 * millimeter;
            caja(context, id + "fin" + ("rib" ~ toString(n)),
                vector(wallX, ySplit, pz - RIB_T / 2),
                vector(px, ysF + EPS, pz + RIB_T / 2));
            n += 1;
        }
    }
    // Labio de centrado: entra LIP_H en la concha trasera, deslizando con ajuste firme por su pared interior
    gota(context, id + "fin" + "lipA", g, cx, wall + definition.fitFirm, ySplit - LIP_H, ySplit + 0.5 * millimeter, ZERO, ZERO, ZERO, ZERO);
    gota(context, id + "fin" + "lipB", g, cx, wall - 0.5 * millimeter, ySplit, ySplit + LIP_T + definition.fitFirm + 0.3 * millimeter, ZERO, ZERO, ZERO, ZERO);
    // Por dentro, el labio nace de la pared con un chaflán de 45° (si no, su base sería una cornisa en voladizo)
    const lipIn = gota(context, id + "lipIn", g, cx, wall + definition.fitFirm + LIP_T, ySplit - LIP_H - 1 * millimeter, ySplit, ZERO, ZERO, ZERO, ZERO);
    const lipIn2 = gota(context, id + "lipIn2", g, cx, wall, ySplit, ySplit + 3 * millimeter, LIP_T + definition.fitFirm, ZERO, ZERO, ZERO);
    subtract(context, id + "lipCut", qUnion([qCreatedBy(id + "fin" + "lipA", EntityType.BODY), qCreatedBy(id + "fin" + "lipB", EntityType.BODY)]), qUnion([lipIn, lipIn2]));
    unite(context, id + "fUnite", qUnion([Of, qCreatedBy(id + "fin", EntityType.BODY)]));
    recortarExterior(context, id + "fClip", bodyAt(id, bodyPoint), g, cx, yw, D, definition);

    // Vaciados de la concha frontal
    // Muesca de la cúpula para la franja del cartucho (abierta por detrás, por arriba y por delante)
    caja(context, id + "fcuts" + "notch",
        vector(cx - sleeveHalf - definition.fitSlide, yw - 1 * millimeter, zNotch - CAP_GAP),
        vector(cx + sleeveHalf + definition.fitSlide, D + 1 * millimeter, zTop + 5 * millimeter));
    for (var i = 0; i < 4; i += 1)
    {
        cylY(context, id + "fcuts" + ("insert" ~ toString(i)), bossPts[i][0], bossPts[i][1], ySplit - 1 * millimeter, ySplit + definition.insertL, definition.insertD / 2);
        cylY(context, id + "fcuts" + ("tip" ~ toString(i)), bossPts[i][0], bossPts[i][1], ySplit + definition.insertL - 0.1 * millimeter,
             ySplit + definition.insertL + TIP_ROOM, definition.screwD / 2);
    }
    // Paso de la clavija USB-C (media abertura en cada concha: muesca abierta hacia el plano de unión, sin puente)
    caja(context, id + "fcuts" + "usb",
        vector(usbX - definition.usbOW / 2, usbY - definition.usbOT / 2, -1 * millimeter),
        vector(usbX + definition.usbOW / 2, usbY + definition.usbOT / 2, zB0 - 2 * millimeter));
    opFillet(context, id + "fcuts" + "usbFillet", {
                "entities" : qParallelEdges(qCreatedBy(id + "fcuts" + "usb", EntityType.EDGE), Z_DIRECTION),
                "radius" : 1.5 * millimeter });
    // Agujeros de los pasadores del frontal
    for (var s in [-1, 1])
    {
        cylY(context, id + "fcuts" + ("pin" ~ toString(s + 1)), cx + s * pinX, pinZ, D - PIN_IN_BODY, D + 1 * millimeter, PIN_D / 2);
    }
    subtract(context, id + "fDoCuts", bodyAt(id, bodyPoint), qCreatedBy(id + "fcuts", EntityType.BODY));

    // ====================== 4. CONCHA TRASERA ======================
    // Se imprime con la cara de pared sobre la cama: todo crece hacia +Y
    const Ob = gota(context, id + "ob", g, cx, ZERO, yw, D, definition.backC, definition.backR, definition.frontC, definition.frontR);
    const Ib = gota(context, id + "ib", g, cx, wall, ZERO, ysF, ciB, riB, ciF, riF);
    subtract(context, id + "bShell", Ob, Ib);
    caja(context, id + "bFrontBox", vector(-1 * millimeter, ySplit, -1 * millimeter), vector(2 * cx + 1 * millimeter, D + 10 * millimeter, zTop + 10 * millimeter));
    subtract(context, id + "bSplit", Ob, qCreatedBy(id + "bFrontBox", EntityType.BODY));
    // Soportes + escuadras en las 4 esquinas de la protoboard (como la tapa de la A)
    var f = 0;
    for (var sx in [-1, 1])
    {
        for (var zs in [[zB0, -1], [zB1, 1]])
        {
            const zEdge = zs[0];
            const sz = zs[1];
            caja(context, id + "bin" + ("f" ~ toString(f)),
                vector(cx + sx * (definition.pcbW / 2 + definition.fitFirm + FENCE_T), -EPS, zEdge + sz * (definition.fitFirm + FENCE_T)),
                vector(cx + sx * (definition.pcbW / 2 - FENCE_IN), definition.pcbY + definition.pcbT, zEdge - sz * FENCE_IN));
            f += 1;
        }
    }
    for (var i = 0; i < 4; i += 1)
    {
        cylY(context, id + "bin" + ("boss" ~ toString(i)), bossPts[i][0], bossPts[i][1], yw, ySplit, definition.bossR);
    }
    // Placa que cierra por dentro el bolsillo de la pletina
    caja(context, id + "bin" + "pad", vector(cx - ph - PAD_T, -EPS, zbP - PAD_T), vector(cx + ph + PAD_T, padTop, ztP + PAD_T));
    unite(context, id + "bUnite", qUnion([Ob, qCreatedBy(id + "bin", EntityType.BODY)]));
    recortarExterior(context, id + "bClip", bodyAt(id, backPoint), g, cx, yw, D, definition);

    // Vaciados de la concha trasera
    caja(context, id + "bcuts" + "pocket",
        vector(cx - definition.pcbW / 2 - definition.fitFirm, definition.pcbY, zB0 - definition.fitFirm),
        vector(cx + definition.pcbW / 2 + definition.fitFirm, definition.pcbY + definition.pcbT + 5 * millimeter, zB1 + definition.fitFirm));
    for (var i = 0; i < 4; i += 1)
    {
        // El fondo del avellanado es un puente circular de una capa que se perfora al montar
        const screwY0 = (definition.cbDepth > 0.01 * millimeter) ? yw + definition.cbDepth + SAC_LAYER : yw - 1 * millimeter;
        cylY(context, id + "bcuts" + ("screw" ~ toString(i)), bossPts[i][0], bossPts[i][1], screwY0, ySplit + 1 * millimeter, definition.screwD / 2);
        if (definition.cbDepth > 0.01 * millimeter)
            cylY(context, id + "bcuts" + ("cb" ~ toString(i)), bossPts[i][0], bossPts[i][1], yw - 1 * millimeter, yw + definition.cbDepth, definition.cbD / 2);
    }
    // Bolsillo de la pletina: perfil (Y, Z) extruido en X. Bisel de 45° arriba (enganche, con holgura
    // perpendicular fitSlide) y chaflán de 45° abajo en el lado profundo, para que el techo no pase de ~10 mm.
    const pkSk = newSketchOnPlane(context, id + "bcuts" + "pkSk", { "sketchPlane" : plane(vector(cx - ph, ZERO, ZERO), X_DIRECTION, Y_DIRECTION) });
    skPolyline(pkSk, "perfil", { "points" : [
                    vector(yw - 1 * millimeter, zbP),
                    vector(yw + dP - PLET_K, zbP),
                    vector(yw + dP, zbP + PLET_K),
                    vector(yw + dP, ztP),
                    vector(yw + definition.pletHook - S2 * definition.fitSlide, ztP),
                    vector(yw + PLET_TIP, zp0 + definition.pletH - definition.pletHook + PLET_TIP + sqrt(2) * definition.fitSlide),
                    vector(yw - 1 * millimeter, zp0 + definition.pletH - definition.pletHook + PLET_TIP + sqrt(2) * definition.fitSlide),
                    vector(yw - 1 * millimeter, zbP)] });
    skSolve(pkSk);
    opExtrude(context, id + "bcuts" + "pletPocket", {
                "entities" : qSketchRegion(id + "bcuts" + "pkSk"),
                "direction" : X_DIRECTION,
                "endBound" : BoundingType.BLIND,
                "endDepth" : 2 * ph });
    opDeleteBodies(context, id + "bcuts" + "pkSkDel", { "entities" : qCreatedBy(id + "bcuts" + "pkSk", EntityType.BODY) });
    caja(context, id + "bcuts" + "usb",
        vector(usbX - definition.usbOW / 2, usbY - definition.usbOT / 2, -1 * millimeter),
        vector(usbX + definition.usbOW / 2, usbY + definition.usbOT / 2, zB0 - 2 * millimeter));
    opFillet(context, id + "bcuts" + "usbFillet", {
                "entities" : qParallelEdges(qCreatedBy(id + "bcuts" + "usb", EntityType.EDGE), Z_DIRECTION),
                "radius" : 1.5 * millimeter });
    // Ranuras de ventilación en la base (largas en Y: en impresión el puente es de 1,6 mm)
    for (var s in [-1, 1])
    {
        const vx = usbX + s * (definition.usbOW / 2 + 3 * millimeter);
        const rv = R1 - wall;
        const dx = abs(vx - cx) + 0.8 * millimeter;
        if (dx < rv - 2 * millimeter)
        {
            caja(context, id + "bcuts" + ("vent" ~ toString(s + 1)),
                vector(vx - 0.8 * millimeter, 6 * millimeter, -1 * millimeter),
                vector(vx + 0.8 * millimeter, 12 * millimeter, z1 - sqrt(rv * rv - dx * dx) + 1 * millimeter));
        }
    }
    subtract(context, id + "bDoCuts", bodyAt(id, backPoint), qCreatedBy(id + "bcuts", EntityType.BODY));
    // Boca del bolsillo de la pletina: chaflán contra la pata de elefante (las únicas rectas en X o Z de esa cama)
    chaflanCama(context, id + "pocketChamfer", bodyAt(id, backPoint), aristasCorte(bodyAt(id, backPoint), plane(vector(ZERO, yw, ZERO), Y_DIRECTION)), Y_DIRECTION, PLET_BED_CH);

    // ====================== 5. CARTUCHO DEL MECANISMO ======================
    // Tumbado sobre su costado -X (plano x = cx - sleeveHalf). Igual que en la A, más la franja de la cúpula.
    caja(context, id + "cart" + "tube",
        vector(cx - sleeveHalf, ysB - SLEEVE_T, zCart0),
        vector(cx + sleeveHalf, ysF, zTop + 5 * millimeter));
    caja(context, id + "cart" + "shelf",
        vector(cx - sleeveHalf, yb, shelfTop - SHELF_T),
        vector(cx + sleeveHalf, ysF, shelfTop));
    caja(context, id + "cart" + "shelf2",
        vector(cx - sleeveHalf, yb, shelf2Bot),
        vector(cx + sleeveHalf, ysF, shelf2Bot + SHELF_T));
    caja(context, id + "cart" + "guide",
        vector(cx - definition.pushW / 2 - definition.fitGuide - GUIDE_T, yGuideB, zc - hP / 2 - definition.fitGuide - GUIDE_T),
        vector(cx + definition.pushW / 2 + definition.fitGuide + GUIDE_T, ysB - SLEEVE_T + EPS, zc + hP / 2 + definition.fitGuide + GUIDE_T));
    unite(context, id + "cartUnite", qCreatedBy(id + "cart", EntityType.BODY));
    // Franja de la cúpula: la piel del canto rodado dentro de la muesca
    const Oc = gota(context, id + "capO", g, cx, ZERO, yw, D, definition.backC, definition.backR, definition.frontC, definition.frontR);
    const Ic = gota(context, id + "capI", g, cx, wall, ZERO, ysF, ciB, riB, ciF, riF);
    subtract(context, id + "capShell", Oc, Ic);
    caja(context, id + "capBand", vector(cx - sleeveHalf, ySplit + CAP_GAP, zNotch), vector(cx + sleeveHalf, D + 1 * millimeter, zTop + 5 * millimeter));
    opBoolean(context, id + "capInt", { "tools" : qUnion([Oc, qCreatedBy(id + "capBand", EntityType.BODY)]), "operationType" : BooleanOperationType.INTERSECTION });
    unite(context, id + "cartUnite2", qUnion([qCreatedBy(id + "cart", EntityType.BODY), qCreatedBy(id + "capO", EntityType.BODY), qCreatedBy(id + "capBand", EntityType.BODY),
                                                qCreatedBy(id + "capInt", EntityType.BODY)]));
    recortarExterior(context, id + "cartClip", bodyAt(id, cartPoint), g, cx, yw, D, definition);

    caja(context, id + "cartCuts" + "slot",
        vector(cx - slotHalf, ysB, zSlotBottom),
        vector(cx + slotHalf, ysS, zTop + 6 * millimeter));
    caja(context, id + "cartCuts" + "guideHole",
        vector(cx - definition.pushW / 2 - definition.fitGuide, yGuideB - 1 * millimeter, zc - hP / 2 - definition.fitGuide),
        vector(cx + definition.pushW / 2 + definition.fitGuide, ysB + 0.05 * millimeter, zc + hP / 2 + definition.fitGuide));
    if (definition.swScrews)
    {
        for (var s in [-1, 1])
        {
            const hx = swX + s * definition.holeS / 2;
            const hy = yb + definition.holeY;
            cylZ(context, id + "cartCuts" + ("swHole" ~ toString(s + 1)), hx, hy,
                 shelfTop - SHELF_T - 1 * millimeter, shelfTop + 0.1 * millimeter, M2_CLEAR / 2);
            cylZ(context, id + "cartCuts" + ("swPilot" ~ toString(s + 1)), hx, hy,
                 shelf2Bot - 0.1 * millimeter, shelf2Bot + SHELF_T + 1 * millimeter, M2_PILOT / 2);
        }
    }
    if (definition.leds)
    {
        for (var s in [-1, 1])
        {
            cylZ(context, id + "cartCuts" + ("led" ~ toString(s + 1)), cx + s * definition.ledS / 2, ledY,
                 shelf2Bot + SHELF_T + 1 * millimeter, zTop + 6 * millimeter, definition.ledD / 2);
        }
    }
    subtract(context, id + "doCartCuts", bodyAt(id, cartPoint), qCreatedBy(id + "cartCuts", EntityType.BODY));

    // Chaflán de entrada de la ranura: aristas de las 4 caras de la ranura que quedan en la cúpula
    const cartFaces = qOwnedByBody(bodyAt(id, cartPoint), EntityType.FACE);
    const slotFaces = qUnion([qCoincidesWithPlane(cartFaces, plane(vector(cx - slotHalf, ZERO, ZERO), X_DIRECTION)),
                              qCoincidesWithPlane(cartFaces, plane(vector(cx + slotHalf, ZERO, ZERO), X_DIRECTION)),
                              qCoincidesWithPlane(cartFaces, plane(vector(ZERO, ysB, ZERO), Y_DIRECTION)),
                              qCoincidesWithPlane(cartFaces, plane(vector(ZERO, ysS, ZERO), Y_DIRECTION))]);
    var entry = [];
    for (var e in evaluateQuery(context, qAdjacent(slotFaces, AdjacencyType.EDGE, EntityType.EDGE)))
    {
        if (evBox3d(context, { "topology" : e }).minCorner[2] > zNotch)
            entry = append(entry, e);
    }
    try silent
    {
        opChamfer(context, id + "slotChamfer", {
                    "entities" : qUnion(entry),
                    "chamferType" : ChamferType.EQUAL_OFFSETS,
                    "width" : SLOT_CHAMFER });
    }
    catch
    {
        reportFeatureWarning(context, id, "No se pudo achaflanar la entrada de la ranura; el resto del modelo es válido.");
    }
    // Chaflán en las aristas rectas que apoyan en la cama (costado -X)
    if (chaflanCama(context, id + "cartBedChamfer", bodyAt(id, cartPoint),
                    qGeometry(qCoincidesWithPlane(qOwnedByBody(bodyAt(id, cartPoint), EntityType.EDGE),
                                                  plane(vector(cx - sleeveHalf, ZERO, ZERO), X_DIRECTION)), GeometryType.LINE), X_DIRECTION, CART_BED_CH) == 0)
        reportFeatureWarning(context, id, "No se pudo achaflanar la base del cartucho; se puede imprimir igual.");

    // ====================== 6. FRONTAL (placa pegada con el QR) ======================
    caja(context, id + "inlay",
        vector(cx - inlayHalf, D, inlayZ0),
        vector(cx + inlayHalf, D + INLAY_T, inlayZ1));
    opFillet(context, id + "inlayFillet", {
                "entities" : qParallelEdges(qCreatedBy(id + "inlay", EntityType.EDGE), Y_DIRECTION),
                "radius" : inlayR });
    try silent
    {
        opChamfer(context, id + "inlayBedChamfer", {
                    "entities" : qCoincidesWithPlane(qOwnedByBody(bodyAt(id, inlayPoint), EntityType.EDGE),
                                                     plane(vector(ZERO, D, ZERO), Y_DIRECTION)),
                    "chamferType" : ChamferType.EQUAL_OFFSETS,
                    "width" : CART_BED_CH });
    }
    catch
    {
        reportFeatureWarning(context, id, "No se pudo achaflanar el canto del frontal; se puede imprimir igual.");
    }
    caja(context, id + "inlayCuts" + "qr",
        vector(cx - qrHalf, D + INLAY_T - definition.qrDepth, definition.qrZ - qrHalf),
        vector(cx + qrHalf, D + INLAY_T + 1 * millimeter, definition.qrZ + qrHalf));
    opFillet(context, id + "inlayCuts" + "qrFillet", {
                "entities" : qParallelEdges(qCreatedBy(id + "inlayCuts" + "qr", EntityType.EDGE), Y_DIRECTION),
                "radius" : min(definition.qrR, qrHalf - 0.1 * millimeter) });
    for (var s in [-1, 1])
    {
        cylY(context, id + "inlayCuts" + ("pin" ~ toString(s + 1)), cx + s * pinX, pinZ,
             D - 1 * millimeter, D + PIN_IN_INLAY, PIN_D / 2);
    }
    subtract(context, id + "doInlayCuts", bodyAt(id, inlayPoint), qCreatedBy(id + "inlayCuts", EntityType.BODY));

    // ====================== 7. EMPUJADOR (igual que en la A) ======================
    const zb = zc - hP / 2;
    const zt = zc + hP / 2;
    const pushSk = newSketchOnPlane(context, id + "pushSk", {
                "sketchPlane" : plane(vector(cx - definition.pushW / 2, ZERO, ZERO), X_DIRECTION, Y_DIRECTION) });
    skPolyline(pushSk, "perfil", { "points" : [
                    vector(yCollarB, zb), vector(ysB, zb), vector(yTip, zb + rampZout),
                    vector(yTip, zt - rampZin), vector(ysB, zt), vector(yCollarB, zt), vector(yCollarB, zb)] });
    skSolve(pushSk);
    opExtrude(context, id + "pushBody", {
                "entities" : qSketchRegion(id + "pushSk"),
                "direction" : X_DIRECTION,
                "endBound" : BoundingType.BLIND,
                "endDepth" : definition.pushW });
    opDeleteBodies(context, id + "pushSkDel", { "entities" : qCreatedBy(id + "pushSk", EntityType.BODY) });
    caja(context, id + "pushCollar",
        vector(cx - definition.pushW / 2 - COLLAR_X, yCollarB, zb),
        vector(cx + definition.pushW / 2 + COLLAR_X, yGuideB, zt));
    unite(context, id + "pushUnite", qUnion([qCreatedBy(id + "pushBody", EntityType.BODY), qCreatedBy(id + "pushCollar", EntityType.BODY)]));
    const pushEdges = qOwnedByBody(bodyAt(id, pushPoint), EntityType.EDGE);
    const tipRound = min(PUSH_ROUND, (tipFlat - 0.3 * millimeter) / (tan(definition.rampIn / 2) + tan(definition.rampOut / 2)));
    try silent
    {
        opFillet(context, id + "pushRound", {
                    "entities" : qUnion([qContainsPoint(pushEdges, vector(cx, ysB, zt)),
                                         qContainsPoint(pushEdges, vector(cx, ysB, zb))]),
                    "radius" : PUSH_ROUND });
        opFillet(context, id + "pushTipRound", {
                    "entities" : qUnion([qContainsPoint(qOwnedByBody(bodyAt(id, pushPoint), EntityType.EDGE), vector(cx, yTip, zt - rampZin)),
                                         qContainsPoint(qOwnedByBody(bodyAt(id, pushPoint), EntityType.EDGE), vector(cx, yTip, zb + rampZout))]),
                    "radius" : tipRound });
    }
    catch
    {
        reportFeatureWarning(context, id, "No se pudo redondear la cuña del empujador; queda en arista viva.");
    }
    opChamfer(context, id + "pushBedChamfer", {
                "entities" : qCoincidesWithPlane(qOwnedByBody(bodyAt(id, pushPoint), EntityType.EDGE),
                                                 plane(vector(ZERO, yCollarB, ZERO), Y_DIRECTION)),
                "chamferType" : ChamferType.EQUAL_OFFSETS,
                "width" : PUSH_BED_CH });

    // ====================== 8. PLETINA DE PARED (dibujada colgada en su sitio) ======================
    // Perfil (Y, Z): canto inferior delantero con chaflán PLET_K; arriba, bisel de 45° hacia la pared (enganche)
    const plSk = newSketchOnPlane(context, id + "plSk", { "sketchPlane" : plane(vector(cx - pw, ZERO, ZERO), X_DIRECTION, Y_DIRECTION) });
    skPolyline(plSk, "perfil", { "points" : [
                    vector(yw, zp0),
                    vector(yw + definition.pletT - PLET_K, zp0),
                    vector(yw + definition.pletT, zp0 + PLET_K),
                    vector(yw + definition.pletT, zp0 + definition.pletH),
                    vector(yw + definition.pletHook, zp0 + definition.pletH),
                    vector(yw, zp0 + definition.pletH - definition.pletHook),
                    vector(yw, zp0)] });
    skSolve(plSk);
    opExtrude(context, id + "plet", {
                "entities" : qSketchRegion(id + "plSk"),
                "direction" : X_DIRECTION,
                "endBound" : BoundingType.BLIND,
                "endDepth" : 2 * pw });
    opDeleteBodies(context, id + "plSkDel", { "entities" : qCreatedBy(id + "plSk", EntityType.BODY) });
    for (var s in [-1, 1])
    {
        const hx = cx + s * definition.pletScrewS / 2;
        const hz = zp0 + zsP;
        cylY(context, id + "plCuts" + ("h" ~ toString(s + 1)), hx, hz, yw - 1 * millimeter, yw + definition.pletT + 1 * millimeter, definition.pletScrewD / 2);
        // Avellanado de 90°: se imprime sobre la cama (cono a 45°)
        fCone(context, id + "plCuts" + ("cs" ~ toString(s + 1)), {
                    "bottomCenter" : vector(hx, yw + definition.pletT - (definition.pletCsD - definition.pletScrewD) / 2, hz),
                    "topCenter" : vector(hx, yw + definition.pletT + 1 * millimeter, hz),
                    "bottomRadius" : definition.pletScrewD / 2,
                    "topRadius" : definition.pletCsD / 2 + 1 * millimeter });
    }
    subtract(context, id + "plDoCuts", bodyAt(id, pletPoint), qCreatedBy(id + "plCuts", EntityType.BODY));
    // Chaflán de cama (la arista del chaflán PLET_K ya es de 45° y chaflanCama la salta)
    const plBed = qGeometry(qCoincidesWithPlane(qOwnedByBody(bodyAt(id, pletPoint), EntityType.EDGE),
                                                plane(vector(ZERO, yw + definition.pletT, ZERO), Y_DIRECTION)), GeometryType.LINE);
    if (chaflanCama(context, id + "plBedChamfer", bodyAt(id, pletPoint), plBed, Y_DIRECTION, PLET_BED_CH) == 0)
        reportFeatureWarning(context, id, "No se pudo achaflanar la base de la pletina; se puede imprimir igual.");

    // ====================== 9. PIEZAS DE PRUEBA (opcional) ======================
    //  0 "Prueba encaje cartucho": parte alta de la concha frontal (muesca, nervio y pared delantera)
    //  1 "Prueba inserto M3":      tetón lateral izquierdo con su trozo de labio (cama: el corte delantero)
    //  2 "Prueba pletina":         bolsillo de la pletina en la concha trasera (se prueba con la pletina real)
    var massInfo = "";
    if (definition.testMode)
    {
        const yCut1 = ySplit + definition.insertL + TIP_ROOM;
        const specs = [
            [bodyPoint, vector(cx - sleeveHalf - 5 * millimeter, ySplit - LIP_H - 1 * millimeter, zCart0 - RIB_SUP_T - 3 * millimeter),
                        vector(cx + sleeveHalf + 5 * millimeter, D + 1 * millimeter, zTop + 5 * millimeter)],
            [bodyPoint, vector(-1 * millimeter, ySplit - LIP_H - 1 * millimeter, sbZ - definition.bossR - 4 * millimeter),
                        vector(cx - sbX + definition.bossR + 3 * millimeter, yCut1, sbZ + definition.bossR + 4 * millimeter)],
            [backPoint, vector(cx - ph - 4 * millimeter, yw - 1 * millimeter, zbP - PAD_T - 3 * millimeter),
                        vector(cx + ph + 4 * millimeter, padTop + 1 * millimeter, ztP + PAD_T + 3 * millimeter)]];
        for (var t = 0; t < size(specs); t += 1)
        {
            const tid = toString(t);
            opPattern(context, id + "test" + ("cp" ~ tid), {
                        "entities" : qSubtraction(bodyAt(id, specs[t][0]), qCreatedBy(id + "test", EntityType.BODY)),
                        "transforms" : [identityTransform()],
                        "instanceNames" : ["c" ~ tid] });
            caja(context, id + "test" + ("box" ~ tid), specs[t][1], specs[t][2]);
            opBoolean(context, id + "test" + ("cut" ~ tid), {
                        "tools" : qUnion([qCreatedBy(id + "test" + ("cp" ~ tid), EntityType.BODY), qCreatedBy(id + "test" + ("box" ~ tid), EntityType.BODY)]),
                        "operationType" : BooleanOperationType.INTERSECTION });
        }
        const couponPts = [vector(cx, D - wallF / 2, zCart0 - RIB_SUP_T - 1 * millimeter),
                           vector(cx - sbX + definition.bossR - 0.6 * millimeter, ySplit + definition.insertL + 1.5 * millimeter, sbZ),
                           vector(cx - ph - 1 * millimeter, yw + 1.5 * millimeter, zbP + 2 * millimeter)];
        const coupon = function(t) returns Query
        {
            return qIntersection([bodyAt(id, couponPts[t]), qCreatedBy(id + "test", EntityType.BODY)]);
        };
        opDeleteBodies(context, id + "test" + "delOrig", {
                    "entities" : qSubtraction(qUnion([bodyAt(id, bodyPoint), bodyAt(id, backPoint), bodyAt(id, inlayPoint)]),
                                              qCreatedBy(id + "test", EntityType.BODY)) });
        const bedEdges = [aristasCorte(coupon(0), plane(vector(ZERO, D, ZERO), Y_DIRECTION)),
                          qCoincidesWithPlane(qOwnedByBody(coupon(1), EntityType.EDGE), plane(vector(ZERO, yCut1, ZERO), Y_DIRECTION)),
                          aristasCorte(coupon(2), plane(vector(ZERO, yw, ZERO), Y_DIRECTION))];
        const names = ["Prueba encaje cartucho", "Prueba inserto M3", "Prueba pletina"];
        for (var t = 0; t < size(names); t += 1)
        {
            try silent
            {
                opChamfer(context, id + "test" + ("ch" ~ toString(t)), {
                            "entities" : bedEdges[t],
                            "chamferType" : ChamferType.EQUAL_OFFSETS,
                            "width" : COUPON_CH });
            }
            catch
            {
                reportFeatureWarning(context, id, "No se pudo achaflanar la base de \"" ~ names[t] ~ "\"; se puede imprimir igual.");
            }
            setProperty(context, { "entities" : coupon(t), "propertyType" : PropertyType.NAME, "value" : names[t] });
        }
        const grams = evVolume(context, { "entities" : qUnion([qCreatedBy(id + "test", EntityType.BODY), bodyAt(id, pushPoint),
                                                               bodyAt(id, cartPoint), bodyAt(id, pletPoint)]) }) / (centimeter ^ 3) * PETG_DENSITY;
        massInfo = "PIEZAS DE PRUEBA (" ~ toString(roundToPrecision(grams, 0)) ~ " g de PETG como mucho) · ";
    }

    // ====================== 10. NOMBRES, EXPLOSIÓN E INFORME ======================
    if (!definition.testMode)
    {
        setProperty(context, { "entities" : bodyAt(id, bodyPoint), "propertyType" : PropertyType.NAME, "value" : "Concha frontal" });
        setProperty(context, { "entities" : bodyAt(id, backPoint), "propertyType" : PropertyType.NAME, "value" : "Concha trasera" });
        setProperty(context, { "entities" : bodyAt(id, inlayPoint), "propertyType" : PropertyType.NAME, "value" : "Frontal" });
    }
    setProperty(context, { "entities" : bodyAt(id, cartPoint), "propertyType" : PropertyType.NAME, "value" : "Cartucho mecanismo" });
    setProperty(context, { "entities" : bodyAt(id, pushPoint), "propertyType" : PropertyType.NAME, "value" : "Empujador" });
    setProperty(context, { "entities" : bodyAt(id, pletPoint), "propertyType" : PropertyType.NAME, "value" : "Pletina de pared" });
    if (definition.explode > 0.01 * millimeter)
    {
        opTransform(context, id + "explodePlet", {
                    "bodies" : bodyAt(id, pletPoint),
                    "transform" : transform(vector(ZERO, -2 * definition.explode, ZERO)) });
        if (!definition.testMode)
        {
            opTransform(context, id + "explodeBack", {
                        "bodies" : bodyAt(id, backPoint),
                        "transform" : transform(vector(ZERO, -definition.explode, ZERO)) });
            opTransform(context, id + "explodeInlay", {
                        "bodies" : bodyAt(id, inlayPoint),
                        "transform" : transform(vector(ZERO, definition.explode, ZERO)) });
        }
    }
    reportFeatureInfo(context, id, massInfo ~ "Exterior (ancho x fondo x alto): " ~ toString(roundToPrecision(2 * cx / millimeter, 1)) ~ " x " ~
                toString(roundToPrecision((D + definition.backT + INLAY_T) / millimeter, 1)) ~ " x " ~ toString(roundToPrecision(zTop / millimeter, 1)) ~
                " mm · union a " ~ toString(roundToPrecision(ySplit / millimeter, 2)) ~ " mm · empujador asoma " ~
                toString(roundToPrecision(pushOut / millimeter, 2)) ~ " mm · palanca " ~ toString(roundToPrecision(leverRest / millimeter, 2)) ~
                " mm en reposo y " ~ toString(roundToPrecision(leverIn / millimeter, 2)) ~ " mm con llavero");
}
//==FUNCS_END
