import Mixin from '@ember/object/mixin';

import {
  BasePredicate,
  SimplePredicate,
  ComplexPredicate,
  DatePredicate,
  StringPredicate,
  DetailPredicate,
} from 'ember-flexberry-data/query/predicate';

export default Mixin.create({
  serializePredicate(predicat) {
    if (!predicat && !(predicat instanceof BasePredicate)) {
      throw new Error(`Wrong predicates`);
    }

    if (predicat instanceof SimplePredicate) {
      return this.serializeSimplePredicate(predicat);
    }

    if (predicat instanceof ComplexPredicate) {
      return this.serializeComplexPredicate(predicat);
    }

    if (predicat instanceof StringPredicate) {
      return this.serializeStringPredicate(predicat);
    }

    if (predicat instanceof DatePredicate) {
      return this.serializeDatePredicate(predicat);
    }

    if (predicat instanceof DetailPredicate) {
      return this.serializeDetailPredicate(predicat);
    }

    return null;
  },

  serializeStringPredicate(predicat) {
    let g = `{StringP|${predicat._attributePath}|${predicat._containsValue}}`;
    return g;
  },

  serializeSimplePredicate(predicat) {
    let valueType = typeof predicat._value,
      g = `{SimpleP|${predicat._attributePath}|${predicat._operator}|${valueType}|${predicat._value}}`;
    return g;
  },

  serializeDatePredicate(predicat) {
    let valueType = typeof predicat._value,
      g = `{DateP|${predicat._attributePath}|${predicat._operator}|${predicat._timeless}|${valueType}|${predicat._value}}`;
    return g;
  },

  serializeComplexPredicate(predicat) {
    let _this = this,
      g = `{ComplexP|${predicat._condition}|${predicat._predicates.map(function (i) {
        return _this.serializePredicate(i);
      }).join('|')}}`;
    return g;
  },

  serializeDetailPredicate(predicat) {
    let _this = this,
      g = `{DetailP|${predicat._detailPath}|${_this.serializePredicate(predicat._predicate)}}`;
    return g;
  },

  deserializePredicate(serializedPredicate) {
    let _this = this,
      slittedVal = [],
      startSymbol = 0,
      endSymbol = 0,
      part = '';
    for (let index = 0; index < serializedPredicate.length; index++) {
      let _symbol = serializedPredicate[index];
      if (_symbol === '{') {
        startSymbol++;
        if (startSymbol - endSymbol === 1) {
          continue;
        }
      }

      if (_symbol === '}') {
        endSymbol++;
        if (startSymbol - endSymbol === 0) {
          continue;
        }
      }

      if (_symbol === '|' && startSymbol - endSymbol === 1) {
        slittedVal.push(part);
        part = '';
        continue;
      }

      part += _symbol;
    }

    slittedVal.push(part);

    let attrCount = slittedVal.get('length');

    if (attrCount < 3) {
      throw new Error(`Wrong string ${serializedPredicate}. Very few attributes`);
    }

    let predicateType = slittedVal[0];

    switch (predicateType) {
      case 'SimpleP':
        if (attrCount === 5) {
          let value = slittedVal[4];
          switch (slittedVal[3]) {
            case 'number':
              if (value === 'NaN') {
                value = null;
              } else {
                value = parseFloat(value);
              }

              break;
            case 'object':
              if (value === 'null' || value === null || value === '(null)') {
                value = null;
              } else {
                throw new Error(`Wrong string ${serializedPredicate}.`);
              }

              break;
            case 'undefined':
            case 'boolean':
            case 'string':
              break;
          }

          return new SimplePredicate(slittedVal[1], slittedVal[2], value);
        }
        throw new Error(`Wrong string ${serializedPredicate}.`);

        break;

      case 'DateP': {
        if (attrCount === 6) {
          let value = slittedVal[5];
          switch (slittedVal[4]) {
            case 'number':
              if (value === 'NaN') {
                value = null;
              } else {
                value = parseFloat(value);
              }

              break;
            case 'object':
            case 'undefined':
            case 'boolean':
            case 'string':
              break;
          }

          return new DatePredicate(slittedVal[1], slittedVal[2], value, slittedVal[3]);
        }
        throw new Error(`Wrong string ${serializedPredicate}.`);

      }
        break;
      case 'ComplexP': {
        if (attrCount >= 4) {
          let condition = slittedVal[1],
            predicates = [];
          slittedVal.forEach(function (attr) {
            if (attr !== predicateType && attr !== condition) {
              predicates.push(_this.deserializePredicate(attr));
            }
          });
          return new ComplexPredicate(condition, ...predicates);
        }
        throw new Error(`Wrong string ${serializedPredicate}. Very few attributes`);

      }
        break;
      case 'DetailP': {
        if (attrCount >= 3) {
          let detailPath = slittedVal[1],
            predicate = _this.deserializePredicate(slittedVal[2]);

          return new DetailPredicate(detailPath).any(predicate);
        }
        throw new Error(`Wrong string ${serializedPredicate}. Very few attributes`);

      }
        break;
      case 'StringP': {
        if (attrCount === 3) {
          return new StringPredicate(slittedVal[1]).contains(slittedVal[2]);
        }
        throw new Error(`Wrong string ${serializedPredicate}. Too much attributes`);

      }
    }
  }
});
