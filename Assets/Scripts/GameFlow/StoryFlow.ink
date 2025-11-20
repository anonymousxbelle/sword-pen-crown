// ============================================
// GLOBAL VARIABLES
// ============================================
VAR show_character_selection = false 
VAR player_choice = ""
VAR kingdom_name = "FILLER"

VAR soldier_name = "SWORD"
VAR king_name = "CROWN"
VAR poet_name = "PEN"

VAR soldier_choice_1 = ""
VAR trust_king = false

VAR feeling_state = ""

// ============================================
// INCLUDE FILES
// ============================================
INCLUDE opening.ink
INCLUDE soldier/soldier_main.ink
INCLUDE poet/poet_main.ink
INCLUDE king/king_main.ink

// ============================================
// START
// ============================================
-> start