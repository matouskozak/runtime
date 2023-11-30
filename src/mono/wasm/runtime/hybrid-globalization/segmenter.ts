import { SegmentationRules } from "./segmentation-rules";

type SegmentationRule = {
    breaks: boolean
    before?: RegExp
    after?: RegExp
}

type SegmentationRuleRaw = {
    breaks: boolean
    before?: string
    after?: string
}
  
type SegmentationTypeTypeRaw = {
    variables: Record<string, string>
    segmentRules: Record<string, SegmentationRuleRaw>
}

/**
 * Replaces variables in the input string with their corresponding values.
 * 
 * @param variables - An object containing key-value pairs of variables and their values.
 * @param input - The string in which variables need to be replaced.
 * @returns The input string with variables replaced by their values.
 * @throws Error if a variable is not found in the variables object.
 */
function replace_variables(variables: Record<string, string>, input: string):string {
    const findVarRegex = /\$[A-Za-z0-9_]+/gm;
    return input.replaceAll(findVarRegex, match => {
        if (!(match in variables)) {
            throw new Error(`No such variable ${match}`)
        }
        return variables[match]
    });
}

function generate_rule_regex (rule: string, variables: Record<string, string>, after: boolean): RegExp {
    return new RegExp(`${after ? '^' : ''}${replace_variables(variables, rule)}${after ? '' : '$'}`);
}


function prepare_segmanation_rules(segmentationTypeValue: SegmentationTypeTypeRaw): Record<string, SegmentationRule> {
    const preparedRules: Record<string, SegmentationRule> = {};

    for (const ruleNr of Object.keys(segmentationTypeValue.segmentRules)) {
        const ruleValue = segmentationTypeValue.segmentRules[ruleNr]
        const preparedRule: SegmentationRule = {
            breaks: ruleValue.breaks,
    }

    if ('before' in ruleValue && ruleValue.before) {
        preparedRule.before = generate_rule_regex(ruleValue.before, segmentationTypeValue.variables, false)
    }
    if ('after' in ruleValue && ruleValue.after) {
        preparedRule.after = generate_rule_regex(ruleValue.after, segmentationTypeValue.variables, true)
    }

        preparedRules[ruleNr] = preparedRule
    }
    return preparedRules
}

// Gets a code point from a UTF-16 string
// handling surrogate pairs appropriately
function get_codepoint(str: string, idx: number) {
    let hi, low;
    const code = str.charCodeAt(idx);
  
    // High surrogate
    if (0xD800 <= code && code <= 0xDBFF) {
        hi = code;
        low = str.charCodeAt(idx + 1);
        if (0xDC00 <= low && low <= 0xDFFF) {
            return ((hi - 0xD800) * 0x400) + (low - 0xDC00) + 0x10000;
        }
  
        return hi;
    }
  
    // Low surrogate
    if (0xDC00 <= code && code <= 0xDFFF) {
        hi = str.charCodeAt(idx - 1);
        low = code;
        if (0xD800 <= hi && hi <= 0xDBFF) {
            return ((hi - 0xD800) * 0x400) + (low - 0xDC00) + 0x10000;
        }
  
        return low;
    }
  
    return code;
}

export class GraphemeBreaker {
    private readonly rules;
    private readonly ruleSortedKeys;

    public constructor() {  
        // Process segmentation rules
        this.rules = prepare_segmanation_rules(SegmentationRules["grapheme"]);
        this.ruleSortedKeys = Object.keys(this.rules).sort((a, b) => Number(a) - Number(b));
    }


    public next_grapheme_break(str: string, startIndex: number): number {
        if (startIndex < 0)
            return 0;
    
        if (startIndex >= str.length - 1)
            return str.length;
    
        let prev = String.fromCharCode(get_codepoint(str, startIndex));
        for (let i = startIndex + 1; i < str.length; i++) {
            // check if is surrogate pair
            let high, low;
            if ((0xd800 <= (high = str.charCodeAt(i - 1)) && high <= 0xdbff) &&
                (0xdc00 <= (low = str.charCodeAt(i)) && low <= 0xdfff)) {
                continue;
            }
    
            const next = String.fromCharCode(get_codepoint(str, i));
            if (this.is_grapheme_break(prev, next))
                return i;
    
            prev = next;
        }
    
        return str.length;
    }

    private is_grapheme_break(prev: string, next: string): boolean {
        // loop through rules and find a match
        for (const ruleKey of this.ruleSortedKeys) {
            const {before, after, breaks} = this.rules[ruleKey];
            // for debugging
            // if (ruleKey === '16' && position === 4) {
            //   console.log({before, after, stringBeforeBreak, stringAfterBreak})
            // }
            if (before) {
                if (!before.test(prev)) {
                    //didn't match the before part, therfore skipping
                    continue;
                }
            }

            if (after) {
                if (!after.test(next)) {
                    //didn't match the after part, therfore skipping
                    continue;
                }
            }

            return breaks;
        }

        //artificial rule 999: if no rule matched is Any ÷ Any so return true
        return true;
    }
}